using System;
using Domain.Entities;

namespace Application.Models.Dtos
{
    public class NodeDto
    {
        public Guid Id { get;  }
        public string Title { get; }
        public string ExternalId { get;  }
            
        public NodeDto( Node node )
        {
            Id = node.Id;
            Title = node.Title;
            ExternalId = node.ExternalId;
        }
    }
}