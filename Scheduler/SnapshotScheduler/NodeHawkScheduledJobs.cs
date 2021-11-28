using System;
using System.Threading.Tasks;
using Application.CommandHandling.Nodes;
using Application.CommandHandling.Nodes.Snapshots;
using MediatR;

namespace Scheduler.SnapshotScheduler
{
    //TODO: make this extend the recurring job manager maybe?
    public class NodeHawkScheduledJobs
    {
        private readonly IMediator _mediator;

        public NodeHawkScheduledJobs( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task SnapshotAll( )
        {
            await _mediator.Send( new SnapshotAll.Command( ) );
        }

        public async Task CleanOldSnapshots( )
        {
            await _mediator.Send( new CleanOldSnapshots.Command { CleanBefore = TimeSpan.FromDays( 7 ) } );
        }
    }
}