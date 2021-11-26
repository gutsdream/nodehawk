using Domain.Entities;
using Xunit;

namespace Domain.Testing.Entities.NodeTests
{
    public class SetConnectionDetails
    {
        private readonly Node _node;

        public SetConnectionDetails( )
        {
            _node = TestData.Create.Node( );
        }

        [Fact]
        public void Should_SetTitle_When_ConnectionDetailsIsNotNull( )
        {
            // Given
            var newConnectionDetails = new ConnectionDetails( "new host", "username", "key" );

            // When
            _node.SetConnectionDetails(newConnectionDetails);

            // Then
            Assert.Equal( newConnectionDetails, _node.ConnectionDetails );
        }

        [Fact]
        public void Should_ThrowException_When_ConnectionDetailsIsNull( )
        {
            // When
            var ex = Record.Exception( ( ) => _node.SetConnectionDetails( null ) );

            // Then
            Assert.NotNull( ex );
        }
    }
}