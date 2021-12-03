using System;
using Application.Core.Shared.Interfaces;

namespace Application.Core.Features.Aws.BackupNode
{
    public class NodeBackedUpEvent : IApplicationEvent
    {
        public Guid NodeId { get; }

        public NodeBackedUpEvent( Guid nodeId )
        {
            NodeId = nodeId;
        }
    }
}