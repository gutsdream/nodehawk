using Domain.ExceptionHandling;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Represents an immutable integer between 0 to 100.
    /// </summary>
    public class Percentage
    {
        public int Value { get; }

        public Percentage( int value )
        {
            Throw.IfNot.Percentage( value, nameof( value ) );
            Value = value;
        }
    }
}