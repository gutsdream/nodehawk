using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.QueryHandling.Nodes
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
                .ToListAsync( );

            // Even if no nodes are found that's not technically invalid, it's a normal state to not have set up any nodes
            return QueryResult<List<NodeDto>>.Found( nodes
                .Select( x => new NodeDto( x ) )
                .ToList( ) );
        }
    }
}