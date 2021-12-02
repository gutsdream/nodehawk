using System;
using Application.Core.Interfaces;
using Domain.Entities;

namespace Application.Core.Models.JobActivities
{
    public class CleanNodeActivity : IJobActivity
    {
        public Guid Id { get; }
        public string Title => $"Cleaning Node: '{_nodeTitle}'";
        public string Status => _status;
        
        private readonly string _nodeTitle;
        private string _status;

        public CleanNodeActivity( Node node )
        {
            Id = Guid.NewGuid( );
            _nodeTitle = node.Title;
        }
        
        public void ConnectingToNode(  )
        {
            _status = "Connecting to Node via SSH...";
        }
        
        public void DeletingDockerOtNodeLogFile(  )
        {
            _status = "Deleting Docker otnode file...";
        }
        
        public void DeletingDockerTextLogs(  )
        {
            _status = "Deleting Docker text logs...";
        }
        
        public void CleaningCacheAndJournals(  )
        {
            _status = "Deleting Cache And Journals...";
        }
    }
}