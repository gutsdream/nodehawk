using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.CommandHandling.Nodes;
using Application.Testing.Mocks;
using Domain.Entities;
using Domain.Testing;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Nodes
{
    public class DeleteNodeHandlerTests
    {
        private readonly DeleteNodeHandler _deleteNodeHandler;

        private readonly List<Node> _nodes;

        private readonly RepositoryMock _repositoryMock;

        public DeleteNodeHandlerTests( )
        {
            _nodes = new List<Node>( );

            _repositoryMock = new RepositoryMock( );
            _repositoryMock.WithEntitiesFor( ( ) => _nodes );

            _deleteNodeHandler = new DeleteNodeHandler( _repositoryMock.Object );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandIsValid_And_NodeIdMatchesExistingNode( )
        {
            // Given
            var id = GivenIdOfNodeIdInRepository( );
            var command = new DeleteNode.Command
            {
                NodeId = id
            };

            // When
            var result = await _deleteNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
            Assert.True( _repositoryMock.DoesNotContain<Node>( x => x.Id == id ) );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_CommandNodeIdIsDefault( )
        {
            // Given
            GivenIdOfNodeIdInRepository( );
            var command = new DeleteNode.Command
            {
                NodeId = default
            };

            // When
            var result = await _deleteNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.NodeId ), "'Node Id' must not be equal to '00000000-0000-0000-0000-000000000000'." );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_NodeDoesNotExistInRepository( )
        {
            // Given
            var id = Guid.NewGuid( );
            var command = new DeleteNode.Command
            {
                NodeId = id
            };

            // When
            var result = await _deleteNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.NodeId ), $"A node with {nameof( command.NodeId )} '{id}' was not found." );
        }

        private Guid GivenIdOfNodeIdInRepository( )
        {
            var node = TestData.Create.Node( );
            _nodes.Add( node );

            return node.Id;
        }
    }
}