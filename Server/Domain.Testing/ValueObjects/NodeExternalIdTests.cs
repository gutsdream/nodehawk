using Domain.ValueObjects;
using Xunit;

namespace Domain.Testing.ValueObjects
{
    public class NodeExternalIdTests
    {
        [Fact]
        public void Should_CreateNodeExternalId_When_StringLengthIs40Characters( )
        {
            // Given
            var idString = new string( 'a', 40 );

            // When
            var externalId = new NodeExternalId( new string( 'a', 40 ) );

            // Then
            Assert.Equal( idString, externalId.Value );
        }

        [Fact]
        public void Should_CreateNodeExternalIdWithNullValue_When_StringIsNull( )
        {
            // When
            var externalId = new NodeExternalId( null );

            // Then
            Assert.NotNull( externalId );
            Assert.Null( externalId.Value );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 39 )]
        [InlineData( 41 )]
        public void Should_ThrowException_When_GivenExternalIdWithLengthLessThanOrGreaterThan40Characters( int stringLength )
        {
            // When
            var exception = Record.Exception( ( ) => new NodeExternalId( new string( 'a', stringLength ) ) );

            // Then
            Assert.NotNull( exception );
        }
    }
}