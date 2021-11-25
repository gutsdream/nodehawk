using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.QueryHandling.Nodes
{
    public class NodeDetails
    {
        public class Query : IRequest<IQueryResult<NodeDto>>
        {
            public Guid Id { get; }

            public Query( Guid id )
            {
                Id = id;
            }
        }
    }

    public class NodeDetailsHandler : IRequestHandler<NodeDetails.Query, IQueryResult<NodeDto>>
    {
        private readonly IRepository _repository;

        public NodeDetailsHandler( IRepository repository )
        {
            _repository = repository;
        }

        public async Task<IQueryResult<NodeDto>> Handle( NodeDetails.Query request, CancellationToken cancellationToken )
        {
            var node = await _repository.Get<Node>( )
                .Include( x => x.Snapshots )
                .FirstOrDefaultAsync( x => x.Id == request.Id );

            return node != null
                ? QueryResult<NodeDto>.Found( new NodeDto( node ) )
                : QueryResult<NodeDto>.NotFound( );
        }
    }
}