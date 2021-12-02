using System;
using Application.Core.Interfaces;
using Domain.Entities;

namespace Application.Core.Models.JobActivities
{
    public class CreateNodeSnapshotActivity : IJobActivity
    {
        public Guid Id { get; }
        public string Title => $"Refreshing information for: '{_nodeTitle}'";
        public string Status => _status;
        
        private readonly string _nodeTitle;
        private string _status;

        public CreateNodeSnapshotActivity( Node node )
        {
            Id = Guid.NewGuid( );
            _nodeTitle = node.Title;
        }
        
        public void ConnectingToNode(  )
        {
            _status = "Connecting to Node via SSH...";
        }
        
        public void CheckingSpaceUsed(  )
        {
            _status = "Getting space used on drive..";
        }
        
        public void CheckingIfNodeOnline(  )
        {
            _status = "Checking if Docker container is running...";
        }
    }
}