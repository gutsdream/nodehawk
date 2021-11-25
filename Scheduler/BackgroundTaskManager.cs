using System;
using System.Threading.Tasks;
using Application.CommandHandling.Nodes.Snapshots;
using Application.Interfaces;
using Hangfire;
using MediatR;

namespace Scheduler
{
    public class BackgroundTaskManager : IBackgroundTaskManager
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMediator _mediator;

        public BackgroundTaskManager( IBackgroundJobClient backgroundJobClient, IMediator mediator )
        {
            _backgroundJobClient = backgroundJobClient;
            _mediator = mediator;
        }

        public void CreateNodeSnapshot( Guid nodeId )
        {
            _backgroundJobClient.Enqueue( ( ) => GenerateSnapshot( nodeId ) );
        }

        public async Task GenerateSnapshot( Guid nodeId )
        {
            await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = nodeId } );
        }
    }
}