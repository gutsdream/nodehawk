using Domain.Entities;
using Xunit;

namespace Domain.Testing.Entities.NodeTests
{
    public class NodeConstructor
    {
        private readonly string _nonNullString = "yeet";

        [Fact]
        public void Should_ConstructNodeSuccessfully_When_GivenValidParameters( )
        {
            // When
            var node = new Node( _nonNullString, CreateConnectionDetails( ) );

            // Then
            Assert.NotNull( node );
            Assert.Equal( _nonNullString, node.Title );
        }

        [Fact]
        public void Should_ThrowException_When_TitleNull( )
        {
            // When
            var exception = Record.Exception( ( ) => new Node( null, CreateConnectionDetails( ) ) );

            // Then
            Assert.NotNull( exception );
        }

        [Fact]
        public void Should_ThrowException_When_ConnectionDetailsNull( )
        {
            // When
            var exception = Record.Exception( ( ) => new Node( _nonNullString, null ) );

            // Then
            Assert.NotNull( exception );
        }

        [Fact]
        public void Should_ConstructNodeSuccessfully_When_GivenExternalIdWithLengthOf40Characters( )
        {
            // When
            var externalId = new string( 'a', 40 );
            var node = new Node( _nonNullString, CreateConnectionDetails( ), externalId );

            // Then
            Assert.NotNull( node );
            Assert.Equal( _nonNullString, node.Title );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 39 )]
        [InlineData( 41 )]
        public void Should_ThrowException_When_GivenExternalIdWithLengthLessThanOrGreaterThan40Characters( int stringLength )
        {
            // When
            var externalId = new string( 'a', stringLength );
            var exception = Record.Exception( ( ) => new Node( _nonNullString, CreateConnectionDetails( ), externalId ) );

            // Then
            Assert.NotNull( exception );
        }

        private ConnectionDetails CreateConnectionDetails( )
        {
            return new ConnectionDetails( _nonNullString, _nonNullString, _nonNullString );
        }
    }
}