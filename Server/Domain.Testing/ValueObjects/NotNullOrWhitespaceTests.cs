using Domain.ValueObjects;
using Xunit;

namespace Domain.Testing.ValueObjects
{
    public class NotNullOrWhitespaceTests
    {
        [Fact]
        public void Should_Create_When_ValueIsNotNullOrWhitespace( )
        {
            // Given
            var value = "hello!";

            // When
            var valueAsNonNull = new NotNullOrWhitespace( value );

            // Then
            Assert.Equal( value, valueAsNonNull.Value );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        [InlineData( "     " )]
        public void Should_ThrowException_When_AccessKeyIsNullOrEmptyOrWhitespace( string value )
        {
            // When
            var exception = Record.Exception( ( ) => new NotNullOrWhitespace( value ) );

            // Then
            Assert.NotNull( exception );
        }
    }
}