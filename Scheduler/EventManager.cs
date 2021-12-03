using System.Threading.Tasks;
using Application.Core.Interfaces;
using Application.Core.Shared.Interfaces;
using Hangfire;
using MediatR;

namespace Scheduler
{
    public class EventManager : IEventManager
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMediator _mediator;

        public EventManager( IBackgroundJobClient backgroundJobClient, IMediator mediator )
        {
            _backgroundJobClient = backgroundJobClient;
            _mediator = mediator;
        }

        public void PublishEvent<TEvent>( TEvent applicationEvent ) where TEvent : IApplicationEvent
        {
            _backgroundJobClient.Enqueue( ( ) => Publish( applicationEvent ) );
            // Background firing of task
            // Task.Run( ( ) => Publish( applicationEvent ) );
        }

        // Must be public for HangFire to consume it
        public async Task Publish<TEvent>( TEvent applicationEvent ) where TEvent : IApplicationEvent
        {
            await _mediator.Publish( applicationEvent );
        }
    }
}