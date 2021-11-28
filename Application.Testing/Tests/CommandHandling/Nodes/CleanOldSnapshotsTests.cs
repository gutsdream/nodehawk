using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.CommandHandling.Nodes;
using Application.Testing.Mocks;
using Domain.Entities;
using Testing.Shared;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Nodes
{
    public class CleanOldSnapshotsTests
    {
        private readonly CleanOldSnapshotsHandler _cleanOldSnapshotsHandler;

        private readonly RepositoryMock _repositoryMock;
        private readonly List<Node.Snapshot> _nodeSnapshots;

        public CleanOldSnapshotsTests( )
        {
            _nodeSnapshots = new List<Node.Snapshot>( );
            _repositoryMock = new RepositoryMock( );
            _repositoryMock.WithEntitiesFor( ( ) => _nodeSnapshots );

            _cleanOldSnapshotsHandler = new CleanOldSnapshotsHandler( _repositoryMock.Object );
        }

        [Fact]
        public async Task ShouldNot_RemoveSnapshots_AfterCleanBeforeDate( )
        {
            // Given
            var node = TestData.Create.Node( );
            node.CreateSnapshot( 50, true );
            var snapshot = node.Snapshots.First( );
            _nodeSnapshots.Add( snapshot );

            // When
            var result = await _cleanOldSnapshotsHandler.Handle( new CleanOldSnapshots.Command { CleanBefore = TimeSpan.FromDays( 7 ) },
                CancellationToken.None );

            // Then
            Assert.True( result.IsSuccessful );
            Assert.True( _repositoryMock.Contains<Node.Snapshot>( x => x.CreatedDateUtc == snapshot.CreatedDateUtc ) );
        }

        [Fact]
        public async Task Should_RemoveSnapshots_BeforeCleanBeforeDate( )
        {
            // Given
            var node = TestData.Create.Node( );
            node.CreateSnapshot( 50, true );
            var snapshot = node.Snapshots.First( );
            _nodeSnapshots.Add( snapshot );

            // When
            // * -1 will set it forward a week
            var result = await _cleanOldSnapshotsHandler.Handle( new CleanOldSnapshots.Command { CleanBefore = TimeSpan.FromDays( 7 ) * -1 },
                CancellationToken.None );

            // Then
            Assert.True( result.IsSuccessful );
            Assert.True( _repositoryMock.DoesNotContain<Node.Snapshot>( x => x.CreatedDateUtc == snapshot.CreatedDateUtc ) );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_CleanBeforeIsDefault( )
        {
            // Given
            var command = new CleanOldSnapshots.Command { CleanBefore = default };

            // When
            // * -1 will set it forward a week
            var result = await _cleanOldSnapshotsHandler.Handle( command, CancellationToken.None );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.CleanBefore ), "'Clean Before' must not be equal to '00:00:00'." );
        }
    }
}