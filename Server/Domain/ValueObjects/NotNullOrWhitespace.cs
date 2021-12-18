using Domain.ExceptionHandling;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Immutable and non nullable wrapper for any class type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class NotNullOrWhitespace
    {
        public string Value { get; }

        public NotNullOrWhitespace( string value )
        {
            Throw.If.NullOrWhitespace( value, nameof( value ) );
            Value = value;
        }
    }
}