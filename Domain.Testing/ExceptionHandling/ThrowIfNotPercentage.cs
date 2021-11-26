using Domain.ExceptionHandling;
using Xunit;

namespace Domain.Testing.ExceptionHandling
{
    public class ThrowIfNotPercentage
    {
        [Theory]
        [InlineData( -1 )]
        [InlineData( 101 )]
        public void Should_Throw_When_ValueIsNotPercentage( int value )
        {
            // When
            var ex = Record.Exception( ( ) => Throw.IfNot.Percentage( value, nameof( value ) ) );

            // Then
            Assert.NotNull( ex );
            Assert.Equal( $"{nameof( value )} must be between 0-100 to represent a valid percentage", ex.Message );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 100 )]
        [InlineData( 50 )]
        public void ShouldNot_Throw_When_ValueIsPercentage( int value )
        {
            // When
            var ex = Record.Exception( ( ) => Throw.IfNot.Percentage( value, nameof( value ) ) );

            // Then
            Assert.Null( ex );
        }
    }
}