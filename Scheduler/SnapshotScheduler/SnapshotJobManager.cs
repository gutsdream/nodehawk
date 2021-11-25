using System.Threading.Tasks;
using Application.CommandHandling.Nodes.Snapshots;
using Hangfire;
using MediatR;

namespace Scheduler.SnapshotScheduler
{
    public class SnapshotJobManager
    {
        private readonly IMediator _mediator;

        public SnapshotJobManager( IMediator mediator  )
        {
            _mediator = mediator;
        }

        //TODO: tidy up
        public async Task InitializeJobs( IRecurringJobManager jobManager )
        {
            // Set a recurring node snapshot job and then do an initial snapshot
            
        }

        public async Task SnapshotAll( )
        {
            await _mediator.Send( new SnapshotAll.Command( ) );
        }
    }
}