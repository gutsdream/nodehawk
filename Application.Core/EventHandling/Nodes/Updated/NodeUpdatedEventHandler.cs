using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.Nodes.Commands.Update;
using Application.Core.Features.SshManagement.Snapshots.Create;
using MediatR;

namespace Application.Core.EventHandling.Nodes.Updated
{
    public class NodeUpdatedEventHandler : INotificationHandler<NodeUpdatedEvent>
    {
        private readonly IMediator _mediator;

        public NodeUpdatedEventHandler( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task Handle( NodeUpdatedEvent notification, CancellationToken cancellationToken )
        {
            await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = notification.NodeId } );
        }
    }
}