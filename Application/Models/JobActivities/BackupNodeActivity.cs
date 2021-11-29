using System;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Models.JobActivities
{
    public class BackupNodeActivity : IJobActivity
    {
        public Guid Id { get; }
        public string Title => $"Backing up Node: '{_nodeTitle}'";
        public string Status => _status;
        
        private readonly string _nodeTitle;
        private string _status;

        public BackupNodeActivity( Node node )
        {
            Id = Guid.NewGuid( );
            _nodeTitle = node.Title;
            _status = "Authenticating with AWS...";
        }
        
        public void ConnectingToNode(  )
        {
            _status = "Connecting to Node via SSH...";
        }

        public void RunningBackupScript( )
        {
            _status = "Running AWS Backup script - Please wait, this can take a while...";
        }
    }
}