using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.CommandHandling.Nodes;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Testing.Shared;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Nodes
{
    public class CleanOldSnapshotsTests
    {
        private CleanOldSnapshotsHandler _cleanOldSnapshotsHandler;

        private DataContext _context;

        [Fact]
        public async Task ShouldNot_RemoveSnapshots_AfterCleanBeforeDate( )
        {
            // Given
            GivenFreshHandler( );

            var node = TestData.Create.Node( );
            node.CreateSnapshot( 50, true );
            var snapshot = node.Snapshots.First( );
            _context.NodeSnapshots.Add( snapshot );

            // When
            var result = await _cleanOldSnapshotsHandler.Handle( new CleanOldSnapshots.Command { CleanBefore = TimeSpan.FromDays( 7 ) },
                CancellationToken.None );

            // Then
            Assert.True( result.IsSuccessful );
            Assert.True( await _context.NodeSnapshots.AnyAsync( x => x.CreatedDateUtc == snapshot.CreatedDateUtc ) );
        }

        [Fact]
        public async Task Should_RemoveSnapshots_BeforeCleanBeforeDate( )
        {
            // Given
            GivenFreshHandler( );

            var node = TestData.Create.Node( );
            node.CreateSnapshot( 50, true );
            var snapshot = node.Snapshots.First( );
            _context.NodeSnapshots.Add( snapshot );
            await _context.SaveChangesAsync( );

            // When
            // * -1 will set it forward a week
            var result = await _cleanOldSnapshotsHandler.Handle( new CleanOldSnapshots.Command { CleanBefore = TimeSpan.FromDays( 7 ) * -1 },
                CancellationToken.None );

            // Then
            Assert.True( result.IsSuccessful );
            Assert.True( !await _context.NodeSnapshots.AnyAsync( x => x.CreatedDateUtc == snapshot.CreatedDateUtc ) );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_CleanBeforeIsDefault( )
        {
            // Given
            GivenFreshHandler( );

            var command = new CleanOldSnapshots.Command { CleanBefore = default };

            // When
            var result = await _cleanOldSnapshotsHandler.Handle( command, CancellationToken.None );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, nameof( command.CleanBefore ), "'Clean Before' must not be equal to '00:00:00'." );
        }

        private void GivenFreshHandler( )
        {
            _context = TestData.Create.UniqueContext( );
            _cleanOldSnapshotsHandler = new CleanOldSnapshotsHandler( _context );
        }
    }
}