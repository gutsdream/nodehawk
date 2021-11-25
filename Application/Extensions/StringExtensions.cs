using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Extensions
{
    public static class StringExtensions
    {
        public static List<string> SplitToList( this string value )
        {
            return value == null 
                ? new List<string>( ) 
                : value.Split( ' ' ).ToList( );
        }

        public static List<string> Ignore( this List<string> values, params string[] valuesToIgnore )
        {
            return values
                .Where( x => !valuesToIgnore.Contains( x ) )
                .ToList( );
        }

        public static string RemoveNonNumericCharacters( this string impureString )
        {
            return impureString == null 
                ? null 
                : new string( impureString.Where( char.IsDigit ).ToArray( ) );
        }

        public static int AsInt( this string numericString )
        {
            return Convert.ToInt32( numericString );
        }

        public static StringListIndexer FindWord( this List<string> values, string target )
        {
            var index = values.IndexOf( target );
            return new StringListIndexer( values, index );
        }
    }

    public class StringListIndexer
    {
        private readonly List<string> _values;
        private int _currentIndex = 0;

        public StringListIndexer( List<string> values, int currentIndex = 0 )
        {
            _values = values;
            _currentIndex = currentIndex;
        }

        public StringListIndexer SkipWords( int wordsToSkip )
        {
            _currentIndex += wordsToSkip;

            return this;
        }

        public string Get( )
        {
            return _values[_currentIndex];
        }
    }
}