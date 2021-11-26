using MediatR;

namespace Application.Interfaces
{
    public interface IBackgroundTaskManager
    {
        void QueueRequest<TRequest, TResult>( TRequest request ) where TRequest : IRequest<TResult>;
    }

    public class QueueableRequest<TResult> : IRequest<TResult>
    {
        
    }
}