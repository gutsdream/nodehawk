using System;
using Domain.Entities;

namespace Application.Core.Models.Dtos
{
    public class NodeDto
    {
        public Guid Id { get;  }
        
        public string Title { get; }
        public string ExternalId { get;  }
        
        public int? SpaceUsedPercentage { get; }
        public bool ContainerRunning { get; }

        public DateTime? LastBackupDateUtc { get; }
        public DateTime? LastCleanedDateUtc { get; }
        public DateTime? LastSnapshotDateUtc { get; }
        
        
        public NodeDto( Node node )
        {
            Id = node.Id;
            Title = node.Title;
            ExternalId = node.ExternalId;
            
            var snapshot = node.MostRecentSnapshot;
            SpaceUsedPercentage = snapshot?.SpaceUsedPercentage;
            ContainerRunning = snapshot?.ContainerRunning ?? false;

            LastBackupDateUtc = node.LastBackupDateUtc;
            LastCleanedDateUtc = node.LastCleanedDateUtc;
            LastSnapshotDateUtc = node.LastSnapshotDateUtc;
        }
    }
}