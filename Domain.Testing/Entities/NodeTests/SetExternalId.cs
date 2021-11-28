using Domain.Entities;
using Testing.Shared;
using Xunit;

namespace Domain.Testing.Entities.NodeTests
{
    public class SetExternalId
    {
        private readonly Node _node;

        public SetExternalId( )
        {
            _node = TestData.Create.Node( );
        }

        [Fact]
        public void Should_SetExternalId_When_ExternalIdNull( )
        {
            // When
            _node.SetExternalId( null );

            // Then
            Assert.Null( _node.ExternalId );
        }

        [Fact]
        public void Should_SetExternalId_When_ExternalIdLengthIs40Characters( )
        {
            // Given
            var newExternalId = new string( 'a', 40 );

            // When
            _node.SetExternalId( newExternalId );

            // Then
            Assert.Equal( newExternalId, _node.ExternalId );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 39 )]
        [InlineData( 41 )]
        public void Should_ThrowException_When_TitleNull( int idLength )
        {
            // Given
            var newExternalId = new string( 'a', idLength );

            // When
            var ex = Record.Exception( ( ) => _node.SetExternalId( newExternalId ) );

            // Then
            Assert.NotNull( ex );
        }
    }
}