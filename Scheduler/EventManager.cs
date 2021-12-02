using System.Threading.Tasks;
using Application.Core.Interfaces;
using Application.Core.Shared.Interfaces;
using Hangfire;
using MediatR;

namespace Scheduler
{
    // TODO: move out of scheduler 
    public class EventManager : IEventManager
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMediator _mediator;

        public EventManager( IBackgroundJobClient backgroundJobClient, IMediator mediator )
        {
            _backgroundJobClient = backgroundJobClient;
            _mediator = mediator;
        }

        public void PublishEvent( IApplicationEvent applicationEvent )
        {
            // var jobQueue = new MediatorJobQueue<TRequest, TResult>( _mediator );
            _backgroundJobClient.Enqueue( ( ) => Publish( applicationEvent ) );
        }

        private async Task Publish( IApplicationEvent applicationEvent )
        {
            await _mediator.Publish( applicationEvent );
        }
    }

    // black magic fuckery allows for generics to be used, otherwise json serialization screams
    // public class MediatorJobQueue<TRequest, TResult> where TRequest : IRequest<TResult>
    // {
    //     private readonly IMediator _mediator;
    //
    //     public MediatorJobQueue( IMediator mediator )
    //     {
    //         _mediator = mediator;
    //     }
    //
    //     public async Task SendMediatrRequest( TRequest request )
    //     {
    //         await _mediator.Send( request );
    //     }
    // }
}