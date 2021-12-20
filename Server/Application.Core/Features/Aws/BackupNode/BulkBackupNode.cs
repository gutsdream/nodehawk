using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Core.Features.Aws.BackupNode
{
    public class BulkBackupNode
    {
        public class Command : IRequest
        {
            public List<Guid> NodeIds { get; set; }
        }
    }

    public class BulkBackupNodeCommandHandler : IRequestHandler<BulkBackupNode.Command>
    {
        private readonly IMediator _mediator;

        public BulkBackupNodeCommandHandler( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle( BulkBackupNode.Command request, CancellationToken cancellationToken )
        {
            foreach ( var nodeId in request.NodeIds )
            {
                await _mediator.Send( new BackupNode.Command { NodeId = nodeId } );
            }

            return Unit.Value;
        }
    }
}