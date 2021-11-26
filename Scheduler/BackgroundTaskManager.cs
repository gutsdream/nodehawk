using System.Threading.Tasks;
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

        public void QueueRequest<TRequest, TResult>( TRequest request ) where TRequest : IRequest<TResult>
        {
            var jobQueue = new MediatorJobQueue<TRequest, TResult>( _mediator );
            _backgroundJobClient.Enqueue( ( ) => jobQueue.SendMediatrRequest( request ) );
        }
    }
    
    // black magic fuckery allows for generics to be used, otherwise json serialization screams
    public class MediatorJobQueue<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        private readonly IMediator _mediator;

        public MediatorJobQueue( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task SendMediatrRequest( TRequest request )
        {
            await _mediator.Send( request );
        }
    }
}