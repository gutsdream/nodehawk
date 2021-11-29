using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.CommandHandling.Snapshots
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
        private readonly IRepository _repository;

        public SnapshotAllCommandHandler( IMediator mediator, IRepository repository )
        {
            _mediator = mediator;
            _repository = repository;
        }
        public async Task<Unit> Handle( SnapshotAll.Command request, CancellationToken cancellationToken )
        {
            var nodes = await _repository.Get<Node>( ).ToListAsync( );
            nodes.ForEach( SnapshotNode );
            
            return Unit.Value;
        }

        private async void SnapshotNode( Node node )
        {
            await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = node.Id } );
        }
    }
}