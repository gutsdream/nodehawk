using Application.Core.Shared.Interfaces;

namespace Application.Core.Interfaces
{
    public interface IEventManager
    {
        /// <summary>
        /// Publishes an event in a fire and forget manner.
        /// </summary>
        void PublishEvent<TEvent>( TEvent applicationEvent ) where TEvent : IApplicationEvent;
    }

}