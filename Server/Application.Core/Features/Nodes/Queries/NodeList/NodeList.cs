using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core.Models.Dtos;
using Application.Core.Models.Results;
using Application.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Nodes.Queries.NodeList
{
    public class NodeList
    {
        public class Query : IRequest<IQueryResult<List<NodeDto>>>
        {
        }
    }

    public class NodeListQueryHandler : IRequestHandler<NodeList.Query, IQueryResult<List<NodeDto>>>
    {
        private readonly DataContext _repository;

        public NodeListQueryHandler( DataContext repository )
        {
            _repository = repository;
        }

        public async Task<IQueryResult<List<NodeDto>>> Handle( NodeList.Query request, CancellationToken cancellationToken )
        {
            var nodes = await _repository.Nodes
                .Include( x => x.Snapshots )
                .OrderBy( x => x.CreatedDateUtc )
                .ToListAsync( );

            // Even if no nodes are found that's not technically invalid, it's a normal state to not have set up any nodes
            return QueryResult<List<NodeDto>>.Found( nodes
                .Select( x => new NodeDto( x ) )
                .ToList( ) );
        }
    }
}