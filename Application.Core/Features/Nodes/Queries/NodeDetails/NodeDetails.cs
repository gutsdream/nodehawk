using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core.Models.Dtos;
using Application.Core.Models.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Nodes.Queries.NodeDetails
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
        private readonly Persistence.DataContext _repository;

        public NodeDetailsHandler( Persistence.DataContext repository )
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