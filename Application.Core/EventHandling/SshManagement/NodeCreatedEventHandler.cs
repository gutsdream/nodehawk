using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.SshManagement.Snapshots.Create;
using Application.Core.Features.SshManagement.SpaceManagement.Clean;
using MediatR;

namespace Application.Core.EventHandling.SshManagement
{
    public class NodeCleanedEventHandler : INotificationHandler<NodeCleanedEvent>
    {
        private readonly IMediator _mediator;

        public NodeCleanedEventHandler( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task Handle( NodeCleanedEvent notification, CancellationToken cancellationToken )
        {
            await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = notification.NodeId } );
        }
    }
}