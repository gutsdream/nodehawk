using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.CommandHandling;
using Application.CommandHandling.Nodes;
using Application.CommandHandling.Nodes.Interfaces;
using Application.CommandHandling.Nodes.Snapshots;
using Application.Testing.Mocks;
using Domain.Entities;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Nodes
{
    public class CreateNodeHandlerTests
    {
        private readonly CreateNodeHandler _createNodeHandler;
        
        private readonly List<Node> _nodes;
        
        private readonly RepositoryMock _repositoryMock;
        private readonly BackgroundTaskManagerMock _backgroundTaskManagerMock;

        public CreateNodeHandlerTests( )
        {
            _nodes = new List<Node>( );

            _repositoryMock = new RepositoryMock( );
            _repositoryMock.WithEntitiesFor( ( ) => _nodes );

            _backgroundTaskManagerMock = new BackgroundTaskManagerMock( );
            _backgroundTaskManagerMock.ConfigureQueue<CreateNodeSnapshot.Command, ICommandResult>(  );
            
            var cypherServiceMock = new CypherServiceMock( );

            _createNodeHandler = new CreateNodeHandler( _repositoryMock.Object,
                cypherServiceMock.Object,
                _backgroundTaskManagerMock.Object );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandIsValid_And_NoExistingNodeMatchesTitleOrExternalId( )
        {
            // Given
            const string title = "Node One";
            var command = new CreateNode.Command
            {
                Title = title,
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
            Assert.True( _repositoryMock.Contains<Node>( x => x.Title == title ) );
            Assert.True( _backgroundTaskManagerMock.ContainsRequestType<CreateNodeSnapshot.Command>( ));
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_When_CommandTitleIsNullOrEmpty( string title )
        {
            // Given
            var command = new CreateNode.Command
            {
                Title = title,
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Title ), $"'{nameof( command.Title )}' must not be empty." );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandExternalIdIsNull( )
        {
            // Given
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.True( result.IsSuccessful );
        }
        
        [Fact]
        public async Task Should_ReturnSuccess_When_CommandExternalIdIs40Characters( )
        {
            // Given
            var externalId = new string( 'a', 40 );
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = externalId,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

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
            var externalId = new string( 'a', characterLength );
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = externalId,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result,
                nameof( command.ExternalId ),
                $"'External Id' must be 40 characters in length. You entered {characterLength} characters." );
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Should_ReturnFailure_WhenCommandHostIsNullOrEmpty( string host )
        {
            // Given
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = null,
                Host = host,
                Username = "username",
                Key = "superSecretPassword"
            };

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Host ), $"'{nameof( command.Host )}' must not be empty." );
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Should_ReturnFailure_WhenCommandUsernameIsNullOrEmpty( string username )
        {
            // Given
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = username,
                Key = "superSecretPassword"
            };

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Username ), $"'{nameof( command.Username )}' must not be empty." );
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Should_ReturnFailure_WhenCommandKeyIsNullOrEmpty( string key )
        {
            // Given
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = key
            };

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Key ), $"'{nameof( command.Key )}' must not be empty." );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ExistingNodeMatchesTitle( )
        {
            // Given
            const string title = "Node One";
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            _nodes.Add( CreateNodeFromCommand( command ) );

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.Title ), $"Node with {nameof( Node.Title )} '{title}' already exists." );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ExistingNodeMatchesExternalId( )
        {
            // Given
            const string externalId = "7cda80c35418f07543fae216cad224ea46dd11eb";
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = externalId,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            _nodes.Add( CreateNodeFromCommand( command ) );

            command.Title = "unique title";

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.ExternalId ), $"Node with {nameof( Node.ExternalId )} '{externalId}' already exists." );
        }

        private static Node CreateNodeFromCommand( IMutateNode command )
        {
            return new Node( command.Title, new ConnectionDetails( command.Host, command.Username, command.Key ), command.ExternalId );
        }
    }
}