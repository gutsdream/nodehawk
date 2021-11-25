using System;
using Domain.Entities;

namespace Application.Models.Dtos
{
    public class NodeDto
    {
        public Guid Id { get;  }
        public string Title { get; }
        public string ExternalId { get;  }
        
        public int? SpaceUsedPercentage { get; }
        public bool ContainerRunning { get; }

        public NodeDto( Node node )
        {
            Id = node.Id;
            Title = node.Title;
            ExternalId = node.ExternalId;
            
            var snapshot = node.MostRecentSnapshot;
            SpaceUsedPercentage = snapshot?.SpaceUsedPercentage;
            ContainerRunning = snapshot?.ContainerRunning ?? false;
        }
    }
}