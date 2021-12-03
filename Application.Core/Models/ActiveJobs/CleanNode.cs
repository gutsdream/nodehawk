using System;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Core.Models.ActiveJobs
{
    // TODO: move these into their respective vertical slices
    public class CleanNode : IActiveJob
    {
        public Guid Id { get; }
        public string Title => $"Cleaning Node: '{_nodeTitle}'";
        public string Status => _status;
        public JobType JobType => JobType.CleanNode;

        private readonly string _nodeTitle;
        private string _status;

        public CleanNode( Node node )
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
            _status = "Deleting Docker otnode log file...";
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