using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.Nodes.Commands.Create;
using Application.Core.Persistence;
using Application.Testing.Mocks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Testing.Shared;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Nodes
{
    public class CreateNodeHandlerTests
    {
        private CreateNodeHandler _createNodeHandler;
        private DataContext _context;
        
        private readonly EventManagerMock _eventManagerMock;

        public CreateNodeHandlerTests( )
        {
            _eventManagerMock = new EventManagerMock( );
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_CommandIsValid_And_NoExistingNodeMatchesTitleOrExternalId( )
        {
            // Given
            GivenFreshHandler( );
            
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
            Assert.True( await _context.Nodes.AnyAsync( x => x.Title == title ) );
            Assert.True( _eventManagerMock.ContainsRequestType<NodeCreatedEvent>( ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_When_CommandTitleIsNullOrEmpty( string title )
        {
            // Given
            GivenFreshHandler( );
            
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
            GivenFreshHandler( );
            
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
            GivenFreshHandler( );
            
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
            GivenFreshHandler( );
            
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
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_WhenCommandHostIsNullOrEmpty( string host )
        {
            // Given
            GivenFreshHandler( );
            
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
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_WhenCommandUsernameIsNullOrEmpty( string username )
        {
            // Given
            GivenFreshHandler( );
            
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
        [InlineData( null )]
        [InlineData( "" )]
        public async Task Should_ReturnFailure_WhenCommandKeyIsNullOrEmpty( string key )
        {
            // Given
            GivenFreshHandler( );
            
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
            GivenFreshHandler( );
            
            const string title = "Node One";
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = null,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            _context.Nodes.Add( CreateNodeFromCommand( command ) );
            await _context.SaveChangesAsync( );

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
            GivenFreshHandler( );
            
            const string externalId = "7cda80c35418f07543fae216cad224ea46dd11eb";
            var command = new CreateNode.Command
            {
                Title = "Node One",
                ExternalId = externalId,
                Host = "host",
                Username = "username",
                Key = "superSecretPassword"
            };

            _context.Nodes.Add( CreateNodeFromCommand( command ) );
            await _context.SaveChangesAsync( );

            command.Title = "unique title";

            // When
            var result = await _createNodeHandler.Handle( command, new CancellationToken( ) );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.ExternalId ), $"Node with {nameof( Node.ExternalId )} '{externalId}' already exists." );
        }

        private static Node CreateNodeFromCommand( CreateNode.Command command )
        {
            return new Node( command.Title, new ConnectionDetails( command.Host, command.Username, command.Key ), command.ExternalId );
        }
        
        private void GivenFreshHandler( )
        {
            _context = TestData.Create.UniqueContext( );

            _createNodeHandler = new CreateNodeHandler( _context,
                new CypherServiceMock( ).Object,
                _eventManagerMock.Object );
            
        }
    }
}