using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Nodes.Queries
{
    public class NodeList
    {
        public class Query : IRequest<IQueryResult<List<NodeDto>>>
        {
        }
    }

    public class NodeListQueryHandler : IRequestHandler<NodeList.Query, IQueryResult<List<NodeDto>>>
    {
        private readonly IRepository _repository;

        public NodeListQueryHandler( IRepository repository )
        {
            _repository = repository;
        }

        public async Task<IQueryResult<List<NodeDto>>> Handle( NodeList.Query request, CancellationToken cancellationToken )
        {
            var nodes = await _repository.Get<Node>( )
                .Include( x => x.ConnectionDetails )
                .ToListAsync( );

            return QueryResult<List<NodeDto>>.Found( nodes
                .Select( x => new NodeDto( x ) )
                .ToList( ) );
        }
    }
}