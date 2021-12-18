using System.Threading;
using System.Threading.Tasks;
using Application.Core.Features.SshManagement.Snapshots.Create;
using Application.Core.Persistence;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.SshManagement.Snapshots.All
{
    public class SnapshotAll
    {
        public class Command : IRequest
        {
        }
    }
    
    public class SnapshotAllCommandHandler :IRequestHandler<SnapshotAll.Command>
    {
        private readonly IMediator _mediator;
        private readonly DataContext _repository;

        public SnapshotAllCommandHandler( IMediator mediator, DataContext repository )
        {
            _mediator = mediator;
            _repository = repository;
        }
        public async Task<Unit> Handle( SnapshotAll.Command request, CancellationToken cancellationToken )
        {
            var nodes = await _repository.Nodes.ToListAsync( );
            nodes.ForEach( SnapshotNode );
            
            return Unit.Value;
        }

        private async void SnapshotNode( Node node )
        {
            await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = node.Id } );
        }
    }
}