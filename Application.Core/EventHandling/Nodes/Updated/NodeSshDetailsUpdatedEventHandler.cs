using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.Nodes.Commands.Update.SshDetails;
using Application.Core.Features.SshManagement.Snapshots.Create;
using MediatR;

namespace Application.Core.EventHandling.Nodes.Updated
{
    public class NodeSshDetailsUpdatedEventHandler : INotificationHandler<NodeSshDetailsUpdatedEvent>
    {
        private readonly IMediator _mediator;

        public NodeSshDetailsUpdatedEventHandler( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task Handle( NodeSshDetailsUpdatedEvent notification, CancellationToken cancellationToken )
        {
            await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = notification.NodeId } );
        }
    }
}