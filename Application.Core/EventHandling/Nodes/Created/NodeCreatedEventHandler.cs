using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.Nodes.Commands.Create;
using Application.Core.Features.SshManagement.Snapshots.Create;
using MediatR;

namespace Application.Core.EventHandling.Nodes.Created
{
    public class NodeCreatedEventHandler : INotificationHandler<NodeCreatedEvent>
    {
        private readonly IMediator _mediator;

        public NodeCreatedEventHandler( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task Handle( NodeCreatedEvent notification, CancellationToken cancellationToken )
        {
            await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = notification.NodeId } );
        }
    }
}