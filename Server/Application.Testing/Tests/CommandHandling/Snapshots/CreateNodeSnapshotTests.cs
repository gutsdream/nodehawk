using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.SshManagement;
using Application.Core.Features.SshManagement.Snapshots.Create;
using Application.Core.Persistence;
using Application.Testing.Mocks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Testing.Shared;
using Xunit;

namespace Application.Testing.Tests.CommandHandling.Snapshots
{
    public class CreateNodeSnapshotTests
    {
        private CreateNodeSnapshotHandler _createNodeSnapshotHandler;
        private DataContext _context;

        private const int SpaceTakenOnDrivePercent = 43;

        [Fact]
        public async Task Should_CreateNodeSnapshot( )
        {
            // Given
            GivenFreshHandler( );

            var node = TestData.Create.Node( );
            await GivenNodeIsPersisted( node );

            // When
            var result = await _createNodeSnapshotHandler.Handle( new CreateNodeSnapshot.Command { NodeId = node.Id }, CancellationToken.None );

            // Then
            Assert.True( result.IsSuccessful );

            var snapshot = node.MostRecentSnapshot;
            Assert.True( snapshot.SpaceUsedPercentage == SpaceTakenOnDrivePercent );
            Assert.True( snapshot.ContainerRunning );

            var job = await _context.FinalizedJobs.FirstOrDefaultAsync( );
            Assert.NotNull( job );
            Assert.Equal( JobType.Snapshot, job.JobType );
            Assert.True( job.WasSuccessful );
        }

        private async Task GivenNodeIsPersisted( Node node )
        {
            _context.Nodes.Add( node );
            await _context.SaveChangesAsync( );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_NodeIdDefault( )
        {
            // Given
            GivenFreshHandler( );

            var node = TestData.Create.Node( );
            await GivenNodeIsPersisted( node );

            // When
            var result = await _createNodeSnapshotHandler.Handle( new CreateNodeSnapshot.Command { NodeId = default }, CancellationToken.None );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, "NodeId", "'Node Id' must not be equal to '00000000-0000-0000-0000-000000000000'." );
        }

        [Fact]
        public async Task Should_ReturnFailure_When_NodeDoesNotExist( )
        {
            // Given
            GivenFreshHandler( );
            var nodeId = Guid.NewGuid( );

            // When
            var result = await _createNodeSnapshotHandler.Handle( new CreateNodeSnapshot.Command { NodeId = nodeId }, CancellationToken.None );

            // Then
            Assert.False( result.IsSuccessful );
            Then.ResultContainsError( result, "NodeId", $"A node with NodeId '{nodeId}' was not found." );
        }

        private void GivenFreshHandler( )
        {
            _context = TestData.Create.UniqueContext( );
            var jobManagerFactory = TestData.Create.JobManagerFactory( _context );
            var nodeHawkClientMock = new NodeHawkSshClientMock( );

            nodeHawkClientMock.CommandShouldReturn( SshConstants.GetSpaceTakenOnDrive,
                $"Filesystem     1K-blocks     Used Available Use% Mounted on/dev/vda1       50633164 21659160  28957620  {SpaceTakenOnDrivePercent}% /" );
            
            nodeHawkClientMock.CommandShouldReturn( SshConstants.CheckIfContainerIsRunning, "true" );

            _createNodeSnapshotHandler = new CreateNodeSnapshotHandler( _context, nodeHawkClientMock.Object, jobManagerFactory );
        }
    }
}