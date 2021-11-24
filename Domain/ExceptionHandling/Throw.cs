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
                    throw new ArgumentNullException( $"{name} must not be null." );
                }
            }

            public static void InvalidLength( string value, string name, int length )
            {
                if ( value.Length != length )
                {
                    throw new ArgumentException( $"{name} must have length of {length}." );
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