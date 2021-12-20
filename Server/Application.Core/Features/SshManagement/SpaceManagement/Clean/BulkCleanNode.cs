using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Core.Features.SshManagement.SpaceManagement.Clean
{
    public class BulkCleanNode
    {
        public class Command : IRequest
        {
            public List<Guid> NodeIds { get; set; }
        }
    }

    public class BulkCleanNodeCommandHandler : IRequestHandler<BulkCleanNode.Command>
    {
        private readonly IMediator _mediator;

        public BulkCleanNodeCommandHandler( IMediator mediator )
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle( BulkCleanNode.Command request, CancellationToken cancellationToken )
        {
            foreach ( var nodeId in request.NodeIds )
            {
                await _mediator.Send( new CleanNode.Command { NodeId = nodeId } );
            }

            return Unit.Value;
        }
    }
}