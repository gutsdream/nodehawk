using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Nodes.Queries
{
    public class NodeList
    {
        public class Query : IRequest<List<Node>> { }
    }

    public class NodeListQueryHandler : IRequestHandler<NodeList.Query, List<Node>>
    {
        private readonly IRepository _repository;

        public NodeListQueryHandler( IRepository repository )
        {
            _repository = repository;
        }
        public async Task<List<Node>> Handle( NodeList.Query request, CancellationToken cancellationToken )
        {
            return await _repository.GetAll<Node>( );
        }
    }
}