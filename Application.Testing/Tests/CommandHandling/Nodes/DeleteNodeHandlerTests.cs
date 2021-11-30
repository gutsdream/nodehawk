using System;
using System.Threading;
using System.Threading.Tasks;
using Application.CommandHandling.Nodes;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Testing.Shared;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Nodes
{
    public class DeleteNodeHandlerTests
    {
        private DeleteNodeHandler _deleteNodeHandler;
        private DataContext _context;

        public DeleteNodeHandlerTests( )
        {
            GivenFreshHandler( );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandIsValid_And_NodeIdMatchesExistingNode( )
        {
            // Given
            var id = await GivenIdOfNodeIdInRepository( );
            var command = new DeleteNode.Command
            {
                NodeId = id
            };

            // When
            var result = await _deleteNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
            Assert.True( !await _context.Nodes.AnyAsync( x => x.Id == id ) );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_CommandNodeIdIsDefault( )
        {
            // Given
            await GivenIdOfNodeIdInRepository( );
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

        private async Task<Guid> GivenIdOfNodeIdInRepository( )
        {
            var node = TestData.Create.Node( );
            _context.Nodes.Add( node );
            await _context.SaveChangesAsync( );

            return node.Id;
        }
        
        private void GivenFreshHandler( )
        {
            _context = TestData.Create.UniqueContext( );
            _deleteNodeHandler = new DeleteNodeHandler( _context );
        }
    }
}