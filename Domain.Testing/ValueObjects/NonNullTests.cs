using Domain.ValueObjects.Generics;
using Xunit;

namespace Domain.Testing.ValueObjects
{
    public class NonNullTests
    {
        [Fact]
        public void Should_Create_When_ValueIsNotNull( )
        {
            // Given
            var value = "hello!";

            // When
            var valueAsNonNull = value.AsNonNull( );

            // Then
            Assert.Equal( value, valueAsNonNull.Value );
        }

        [Fact]
        public void Should_ThrowException_When_ValueIsNull( )
        {
            // When
            var exception = Record.Exception( ( ) => new NonNull<string>( null ) );

            // Then
            Assert.NotNull( exception );
        }
    }
}