using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.CommandHandling;
using Application.CommandHandling.Nodes;
using Application.CommandHandling.Nodes.Interfaces;
using Application.CommandHandling.Nodes.Snapshots;
using Application.Testing.Mocks;
using Domain.Entities;
using Domain.Testing;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Nodes
{
    public class UpdateNodeHandlerTests
    {
        private readonly UpdateNodeHandler _updateNodeHandler;

        private readonly List<Node> _nodes;

        private readonly BackgroundTaskManagerMock _backgroundTaskManagerMock;

        public UpdateNodeHandlerTests( )
        {
            _nodes = new List<Node>( );

            var repositoryMock = new RepositoryMock( );
            repositoryMock.WithEntitiesFor( ( ) => _nodes );

            _backgroundTaskManagerMock = new BackgroundTaskManagerMock( );
            _backgroundTaskManagerMock.ConfigureQueue<CreateNodeSnapshot.Command, ICommandResult>( );

            var cypherServiceMock = new CypherServiceMock( );

            _updateNodeHandler = new UpdateNodeHandler( repositoryMock.Object,
                cypherServiceMock.Object,
                _backgroundTaskManagerMock.Object );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandIsValid_And_NoExistingNodeMatchesTitleOrExternalId( )
        {
            // Given
            var guid = GivenIdOfNodeIdInRepository( );
            const string title = "Node One";
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = title,
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
            Assert.True( _backgroundTaskManagerMock.ContainsRequestType<CreateNodeSnapshot.Command>( ) );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_CommandNodeIdIsDefault( )
        {
            // Given
            var command = new UpdateNode.Command
            {
                NodeId = default,
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

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
            var guid = GivenIdOfNodeIdInRepository( );
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = title,
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Title ), $"'{nameof( command.Title )}' must not be empty." );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandExternalIdIsNull( )
        {
            // Given
            var guid = GivenIdOfNodeIdInRepository( );
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandExternalIdIs40Characters( )
        {
            // Given
            var guid = GivenIdOfNodeIdInRepository( );
            var externalId = new string( 'a', 40 );
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = externalId,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

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
            var guid = GivenIdOfNodeIdInRepository( );
            var externalId = new string( 'a', characterLength );
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = externalId,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result,
                nameof( command.ExternalId ),
                $"'External Id' must be 40 characters in length. You entered {characterLength} characters." );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_WhenCommandHostIsNullOrEmpty( string host )
        {
            // Given
            var guid = GivenIdOfNodeIdInRepository( );
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = null,
                Host = host,
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

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
            var guid = GivenIdOfNodeIdInRepository( );
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = username,
                Key = "superSecretPassword"
            };

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

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
            var guid = GivenIdOfNodeIdInRepository( );
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = key
            };

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Key ), $"'{nameof( command.Key )}' must not be empty." );
        }
        
        [Fact]
        public async Task Should_ReturnFailure_When_NodeIdDoesNotMatchExistingNode( )
        {
            // Given
            GivenIdOfNodeIdInRepository( );
            var guid = Guid.NewGuid( );
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            _nodes.Add( UpdateNodeFromCommand( command ) );

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.NodeId ), $"A node with {nameof( Node.Id )} '{guid}' was not found.");
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ExistingNodeMatchesTitle( )
        {
            // Given
            var guid = GivenIdOfNodeIdInRepository( );
            const string title = "Node One";
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            _nodes.Add( UpdateNodeFromCommand( command ) );

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Title ), $"Different node with {nameof( Node.Title )} '{title}' already exists." );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ExistingNodeMatchesExternalId( )
        {
            // Given
            var guid = GivenIdOfNodeIdInRepository( );
            const string externalId = "7cda80c35418f07543fae216cad224ea46dd11eb";
            var command = new UpdateNode.Command
            {
                NodeId = guid,
                Title = "Node One",
                ExternalId = externalId,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            _nodes.Add( UpdateNodeFromCommand( command ) );

            command.Title = "unique title";

            // When
            var result = await _updateNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.ExternalId ), $"Different node with {nameof( Node.ExternalId )} '{externalId}' already exists." );
        }

        private static Node UpdateNodeFromCommand( IMutateNode command )
        {
            return new Node( command.Title, new ConnectionDetails( command.Host, command.Username, command.Key ), command.ExternalId );
        }

        private Guid GivenIdOfNodeIdInRepository( )
        {
            var node = TestData.Create.Node( );
            _nodes.Add( node );
            
            return node.Id;
        }
    }
}