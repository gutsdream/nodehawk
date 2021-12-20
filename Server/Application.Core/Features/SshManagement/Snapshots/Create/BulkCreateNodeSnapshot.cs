using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Core.Features.SshManagement.Snapshots.Create
{
    public class BulkCreateNodeSnapshot
    {
        public class Command : IRequest
        {
            public List<Guid> NodeIds { get; set; }
        }
    }

    public class BulkCreateNodeSnapshotCommandHandler : IRequestHandler<BulkCreateNodeSnapshot.Command>
    {
        private readonly IMediator _mediator;

        public BulkCreateNodeSnapshotCommandHandler( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle( BulkCreateNodeSnapshot.Command request, CancellationToken cancellationToken )
        {
            foreach ( var nodeId in request.NodeIds )
            {
                await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = nodeId } );
            }

            return Unit.Value;
        }
    }
}