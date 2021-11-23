using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Nodes.Queries
{
    public class NodeDetails
    {
        public class Query : IRequest<Node>
        {
            public Guid Id { get; }

            public Query( Guid id )
            {
                Id = id;
            }
        }
    }

    public class NodeDetailsHandler : IRequestHandler<NodeDetails.Query, Node>
    {
        private readonly IRepository _repository;

        public NodeDetailsHandler( IRepository repository )
        {
            _repository = repository;
        }

        public async Task<Node> Handle( NodeDetails.Query request, CancellationToken cancellationToken )
        {
            return await _repository.GetFirstOrDefault<Node>( node => node.Id == request.Id );
        }
    }
}