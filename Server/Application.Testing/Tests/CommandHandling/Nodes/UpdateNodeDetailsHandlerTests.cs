using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.Nodes.Commands.Update.Details;
using Domain.Entities;
using Application.Core.Persistence;
using Application.Testing.Mocks;
using Domain.ValueObjects;
using Domain.ValueObjects.Generics;
using Testing.Shared;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Nodes
{
    public class UpdateNodeDetailsHandlerTests
    {
        private UpdateNodeDetails.UpdateNodeDetailsHandler _updateNodeDetailsHandler;

        private readonly EventManagerMock _eventManagerMock;
        private DataContext _context;

        public UpdateNodeDetailsHandlerTests( )
        {
            _eventManagerMock = new EventManagerMock( );

        }
        
        [Fact]
        public async Task Should_ReturnSuccess_When_CommandIsValid_And_NoExistingNodeMatchesTitleOrExternalId( )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            const string title = "Node One";
            var command = new UpdateNodeDetails.Command
            {
                NodeId = guid,
                Title = title,
                ExternalId = null
            };

            // When
            var result = await _updateNodeDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_CommandNodeIdIsDefault( )
        {
            // Given
            GivenFreshHandler( );
            
            var command = new UpdateNodeDetails.Command
            {
                NodeId = default,
                Title = "Node One",
                ExternalId = null
            };

            // When
            var result = await _updateNodeDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.NodeId ), $"'Node Id' must not be equal to '00000000-0000-0000-0000-000000000000'." );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_When_CommandTitleIsNullOrEmpty( string title )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            var command = new UpdateNodeDetails.Command
            {
                NodeId = guid,
                Title = title,
                ExternalId = null
            };

            // When
            var result = await _updateNodeDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Title ), $"'{nameof( command.Title )}' must not be empty." );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandExternalIdIsNull( )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            var command = new UpdateNodeDetails.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = null
            };

            // When
            var result = await _updateNodeDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandExternalIdIs40Characters( )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            var externalId = new string( 'a', 40 );
            var command = new UpdateNodeDetails.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = externalId,
            };

            // When
            var result = await _updateNodeDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 39 )]
        [InlineData( 41 )]
        public async Task Should_ReturnFailure_When_CommandExternalIdIsLessThanOrGreaterThan40Characters( int characterLength )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            var externalId = new string( 'a', characterLength );
            var command = new UpdateNodeDetails.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = externalId
            };

            // When
            var result = await _updateNodeDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result,
                nameof( command.ExternalId ),
                $"'External Id' must be 40 characters in length. You entered {characterLength} characters." );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_NodeIdDoesNotMatchExistingNode( )
        {
            // Given
            GivenFreshHandler( );
            await GivenIdOfNodeIdInRepository( );
            
            var guid = Guid.NewGuid( );
            var command = new UpdateNodeDetails.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = null,
            };

            _context.Nodes.Add( UpdateNodeFromCommand( command ) );
            await _context.SaveChangesAsync( );

            // When
            var result = await _updateNodeDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.NodeId ), $"A node with {nameof( Node.Id )} '{guid}' was not found.");
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ExistingNodeMatchesTitle( )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            const string title = "Node One";
            var command = new UpdateNodeDetails.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = null,
            };

            _context.Nodes.Add( UpdateNodeFromCommand( command ) );
            await _context.SaveChangesAsync( );

            // When
            var result = await _updateNodeDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Title ), $"Different node with {nameof( Node.Title )} '{title}' already exists." );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ExistingNodeMatchesExternalId( )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            const string externalId = "7cda80c35418f07543fae216cad224ea46dd11eb";
            var command = new UpdateNodeDetails.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = externalId
            };

            _context.Nodes.Add( UpdateNodeFromCommand( command ) );
            await _context.SaveChangesAsync( );

            command.Title = "unique title";

            // When
            var result = await _updateNodeDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.ExternalId ), $"Different node with {nameof( Node.ExternalId )} '{externalId}' already exists." );
        }

        private static Node UpdateNodeFromCommand( UpdateNodeDetails.Command command )
        {
            return new Node( command.Title.AsNonNull( ),
                new ConnectionDetails( "host".AsNonNull( ), "username".AsNonNull( ), "key".AsNonNull( ) ).AsNonNull( ),
                new NodeExternalId( command.ExternalId ) );
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

            _updateNodeDetailsHandler = new UpdateNodeDetails.UpdateNodeDetailsHandler( _context,
                _eventManagerMock.Object );
        }
    }
}