using System;
using Application.Core.Shared.Interfaces;

namespace Application.Core.Features.Nodes.Commands.Update.SshDetails
{
    public class NodeSshDetailsUpdatedEvent : IApplicationEvent
    {
        public Guid NodeId { get; }

        public NodeSshDetailsUpdatedEvent( Guid nodeId )
        {
            NodeId = nodeId;
        }
    }
}