using Domain.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Generics;
using Xunit;

namespace Domain.Testing.Entities.NodeTests
{
    public class NodeConstructor
    {
        [Fact]
        public void Should_ConstructNodeSuccessfully_When_GivenValidParameters( )
        {
            // Given
            const string nodeTitle = "Node One";

            // When
            var node = new Node( nodeTitle.AsNonNull( ), CreateConnectionDetails( ).AsNonNull( ), new NodeExternalId( null ) );

            // Then
            Assert.NotNull( node );
            Assert.Equal( nodeTitle, node.Title );
        }

        private ConnectionDetails CreateConnectionDetails( )
        {
            var nonNullString = "yeet".AsNonNull( );
            return new ConnectionDetails( nonNullString, nonNullString, nonNullString );
        }
    }
}