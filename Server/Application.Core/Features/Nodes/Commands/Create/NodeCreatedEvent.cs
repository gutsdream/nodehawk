using System;
using Application.Core.Shared.Interfaces;

namespace Application.Core.Features.Nodes.Commands.Create
{
    public class NodeCreatedEvent : IApplicationEvent
    {
        public Guid NodeId { get; }

        public NodeCreatedEvent( Guid nodeId )
        {
            NodeId = nodeId;
        }
    }
}