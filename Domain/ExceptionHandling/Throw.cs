using System;

namespace Domain.ExceptionHandling
{
    public static class Throw
    {
        public static class If
        {
            public static void Null( dynamic nullable, string name )
            {
                if ( nullable == null )
                {
                    throw new ArgumentNullException( name );
                }
            }

            public static void InvalidLength( string value, string name, int expectedLength )
            {
                if ( value == null || value.Length != expectedLength )
                {
                    throw new ArgumentException( $"{name} must have length of {expectedLength}." );
                }
            }
        }

        public static class IfNot
        {
            public static void Percentage( int percentage, string name )
            {
                if ( percentage is < 0 or > 100 )
                {
                    throw new ArgumentException( $"{name} must be between 0-100 to represent a valid percentage" );
                }
            }
        }
    }
}