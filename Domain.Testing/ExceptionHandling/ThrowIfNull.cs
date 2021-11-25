using Domain.ExceptionHandling;
using Xunit;

namespace Domain.Testing.ExceptionHandling
{
    public class ThrowIfNull
    {
        [Fact]
        public void Should_ThrowException_When_ValueNull( )
        {
            // Given
            string value = null;

            // When
            var exception = Record.Exception( ( ) => Throw.If.Null( value, nameof( value ) ) );

            // Then
            Assert.NotNull( exception );
            Assert.Equal( $"Value cannot be null. (Parameter '{nameof( value )}')", exception.Message );
        }

        [Fact]
        public void ShouldNot_ThrowException_When_ValueIsNotNull( )
        {
            // Given
            string value = "beep";

            // When
            var exception = Record.Exception( ( ) => Throw.If.Null( value, nameof( value ) ) );

            // Then
            Assert.Null( exception );
        }
    }
}