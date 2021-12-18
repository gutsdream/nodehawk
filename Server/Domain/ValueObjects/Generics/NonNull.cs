using Domain.ExceptionHandling;

namespace Domain.ValueObjects.Generics
{
    /// <summary>
    /// Immutable and non nullable wrapper for any class type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class NonNull<TValue> where TValue : class
    {
        public TValue Value { get; }

        public NonNull( TValue value )
        {
            Throw.If.Null( value, nameof( value ) );
            Value = value;
        }
    }

    public static class NonNullExtensions
    {
        public static NonNull<TValue> AsNonNull<TValue>( this TValue value ) where TValue : class
        {
            return new NonNull<TValue>( value );
        }
    }
}