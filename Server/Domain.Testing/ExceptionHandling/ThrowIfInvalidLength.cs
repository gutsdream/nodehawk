using Domain.ExceptionHandling;
using Xunit;

namespace Domain.Testing.ExceptionHandling
{
    public class ThrowIfInvalidLength
    {
        [Fact]
        public void Should_ThrowException_When_ValueIsNotExpectedLength( )
        {
            // Given
            const int actualLength = 10;
            const int expectedLength = actualLength + 1;

            var value = new string( 'a', actualLength );

            // When
            var ex = Record.Exception( ( ) => Throw.If.InvalidLength( value, nameof( value ), expectedLength ) );

            // Then
            Assert.NotNull( ex );
            Assert.Equal( $"{nameof( value )} must have length of {expectedLength}.", ex.Message );
        }

        [Fact]
        public void ShouldNot_ThrowException_When_ValueIsExpectedLength( )
        {
            // Given
            const int actualLength = 10;

            var value = new string( 'a', actualLength );

            // When
            var ex = Record.Exception( ( ) => Throw.If.InvalidLength( value, nameof( value ), actualLength ) );

            // Then
            Assert.Null( ex );
        }

        [Fact]
        public void Should_ThrowException_When_ValueNull( )
        {
            // Given
            const int actualLength = 10;

            string value = null;

            // When
            var ex = Record.Exception( ( ) => Throw.If.InvalidLength( value, nameof( value ), actualLength ) );

            // Then
            Assert.NotNull( ex );
            Assert.Equal( $"{nameof( value )} must have length of {actualLength}.", ex.Message );
        }
    }
}