using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

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
        private readonly DataContext _repository;

        public NodeDetailsHandler( DataContext repository )
        {
            _repository = repository;
        }

        public async Task<IQueryResult<NodeDto>> Handle( NodeDetails.Query request, CancellationToken cancellationToken )
        {
            var node = await _repository.Nodes
                .Include( x => x.Snapshots )
                .FirstOrDefaultAsync( x => x.Id == request.Id );

            return node != null
                ? QueryResult<NodeDto>.Found( new NodeDto( node ) )
                : QueryResult<NodeDto>.NotFound( );
        }
    }
}