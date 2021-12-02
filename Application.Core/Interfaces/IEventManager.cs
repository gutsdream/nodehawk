using Application.Core.Shared.Interfaces;
using MediatR;

namespace Application.Core.Interfaces
{
    public interface IEventManager
    {
        void PublishEvent( IApplicationEvent applicationEvent );
    }

}