using System;

namespace Domain.ExceptionHandling
{
    public static class Throw
    {
        public static void IfNull( dynamic nullable, string name )
        {
            if ( nullable == null )
            {
                throw new ArgumentNullException( $"{name} must not be null." );
            }
        }

        public static class IfNot
        {
            public static void Percentage( int percentage, string name )
            {
                if ( percentage < 0 || percentage > 100 )
                {
                    throw new ArgumentException( $"{nameof( name )}Must be between 0-100 to represent a valid percentage" );
                }
            }
        }
    }
}