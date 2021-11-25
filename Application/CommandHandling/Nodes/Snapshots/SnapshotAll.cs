using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.CommandHandling.Nodes.Snapshots
{
    public class SnapshotAll
    {
        public class Command : IRequest<ICommandResult>
        {
        }
    }
    
    public class SnapshotAllCommandHandler :IRequestHandler<SnapshotAll.Command, ICommandResult>
    {
        private readonly IMediator _mediator;
        private readonly IRepository _repository;

        public SnapshotAllCommandHandler( IMediator mediator, IRepository repository  )
        {
            _mediator = mediator;
            _repository = repository;
        }
        public async Task<ICommandResult> Handle( SnapshotAll.Command request, CancellationToken cancellationToken )
        {
            var nodes = await _repository.Get<Node>( ).ToListAsync( );
            var errors = new List<Error>( );
            foreach ( var node in nodes )
            {
                var result = await _mediator.Send( new CreateNodeSnapshot.Command { NodeId = node.Id } );
                errors.AddRange(result.Errors);
            }

            var commandResult = new CommandResult( );
            foreach ( var error in errors )
            {
                commandResult.AddError(error);
            }

            return commandResult;
        }
    }
}