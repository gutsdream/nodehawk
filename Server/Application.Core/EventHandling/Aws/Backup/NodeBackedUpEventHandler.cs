using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.Aws.BackupNode;
using Application.Core.Features.SshManagement.Snapshots.Create;
using MediatR;

namespace Application.Core.EventHandling.Aws.Backup
{
    public class NodeBackedUpEventHandler : INotificationHandler<NodeBackedUpEvent>
    {
        private readonly IMediator _mediator;

        public NodeBackedUpEventHandler( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task Handle( NodeBackedUpEvent notification, CancellationToken cancellationToken )
        {
            await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = notification.NodeId } );
        }
    }
}