using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.Nodes.Commands.Update.SshDetails;
using Domain.Entities;
using Application.Core.Persistence;
using Application.Testing.Mocks;
using Testing.Shared;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Nodes
{
    public class UpdateNodeSshDetailsHandlerTests
    {
        private UpdateNodeSshDetailsHandler _updateNodeSshDetailsHandler;
        private DataContext _context;

        private readonly EventManagerMock _eventManagerMock;
        private readonly NodeHawkSshClientMock _sshClientMock;

        public UpdateNodeSshDetailsHandlerTests( )
        {
            _eventManagerMock = new EventManagerMock( );
            _sshClientMock = new NodeHawkSshClientMock( );
        }
        
        [Fact]
        public async Task Should_ReturnSuccess_When_CommandIsValid_And_NoExistingNodeMatchesTitleOrExternalId( )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            var command = new UpdateNodeSshDetails.Command
            {
                NodeId = guid,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeSshDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
            Assert.True( _eventManagerMock.ContainsRequestType<NodeSshDetailsUpdatedEvent>( ) );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_CommandNodeIdIsDefault( )
        {
            // Given
            GivenFreshHandler( );
            
            var command = new UpdateNodeSshDetails.Command
            {
                NodeId = default,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeSshDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.NodeId ), $"'Node Id' must not be equal to '00000000-0000-0000-0000-000000000000'." );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_WhenCommandHostIsNullOrEmpty( string host )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            var command = new UpdateNodeSshDetails.Command
            {
                NodeId = guid,
                Host = host,
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeSshDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Host ), $"'{nameof( command.Host )}' must not be empty." );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_WhenCommandUsernameIsNullOrEmpty( string username )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            var command = new UpdateNodeSshDetails.Command
            {
                NodeId = guid,
                Host = "host",
                Username = username,
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeSshDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Username ), $"'{nameof( command.Username )}' must not be empty." );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_WhenCommandKeyIsNullOrEmpty( string key )
        {
            // Given
            GivenFreshHandler( );
            
            var guid = await GivenIdOfNodeIdInRepository( );
            var command = new UpdateNodeSshDetails.Command
            {
                NodeId = guid,
                Host = "host",
                Username = "username",
                Key = key
            };

            // When
            var result = await _updateNodeSshDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Key ), $"'{nameof( command.Key )}' must not be empty." );
        }
        
        [Fact]
        public async Task Should_ReturnFailure_When_NodeIdDoesNotMatchExistingNode( )
        {
            // Given
            GivenFreshHandler( );
            await GivenIdOfNodeIdInRepository( );
            
            var guid = Guid.NewGuid( );
            var command = new UpdateNodeSshDetails.Command
            {
                NodeId = guid,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            _context.Nodes.Add( UpdateNodeFromCommand( command ) );
            await _context.SaveChangesAsync( );

            // When
            var result = await _updateNodeSshDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.NodeId ), $"A node with {nameof( Node.Id )} '{guid}' was not found.");
        }
        
        [Fact]
        public async Task Should_ReturnFailure_When_SshAuthenticationFails( )
        {
            // Given
            _sshClientMock.SshAuthenticationShouldFail();
            GivenFreshHandler( );
            var guid = await GivenIdOfNodeIdInRepository( );
            
            var command = new UpdateNodeSshDetails.Command
            {
                NodeId = guid,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeSshDetailsHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, "SshDetails", "Could not verify authenticity of provided SSH connection details. Please ensure you have entered them correctly."  );
        }

        private static Node UpdateNodeFromCommand( UpdateNodeSshDetails.Command command )
        {
            return new Node( "placeholdertitle", new ConnectionDetails( command.Host, command.Username, command.Key ) );
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
            var cypherServiceMock = new CypherServiceMock( );

            _context = TestData.Create.UniqueContext( );

            _updateNodeSshDetailsHandler = new UpdateNodeSshDetailsHandler( _context,
                cypherServiceMock.Object,
                _eventManagerMock.Object,
                _sshClientMock.Object );
        }
    }
}