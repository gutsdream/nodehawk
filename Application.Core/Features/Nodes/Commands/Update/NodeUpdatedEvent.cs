using System;
using Application.Core.Shared.Interfaces;

namespace Application.Core.Features.Nodes.Commands.Update
{
    public class NodeUpdatedEvent : IApplicationEvent
    {
        public Guid NodeId { get; }

        public NodeUpdatedEvent( Guid nodeId )
        {
            NodeId = nodeId;
        }
    }
}