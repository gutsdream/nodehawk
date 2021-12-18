using System;
using Application.Core.Shared.Interfaces;

namespace Application.Core.Features.SshManagement.SpaceManagement.Clean
{
    public class NodeCleanedEvent : IApplicationEvent
    {
        public Guid NodeId { get; }

        public NodeCleanedEvent( Guid nodeId )
        {
            NodeId = nodeId;
        }
    }
}