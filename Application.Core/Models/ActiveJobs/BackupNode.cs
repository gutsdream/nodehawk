using System;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Core.Models.ActiveJobs
{
    public class BackupNode : IActiveJob
    {
        public Guid Id { get; }
        public string Title => $"Backing up Node: '{_nodeTitle}'";
        public string Status => _status;
        public JobType JobType => JobType.Backup;

        private readonly string _nodeTitle;
        private string _status;

        public BackupNode( Node node )
        {
            Id = Guid.NewGuid( );
            _nodeTitle = node.Title;
            _status = "Authenticating with AWS...";
        }
        
        public void ConnectingToNode(  )
        {
            _status = "Connecting to Node via SSH...";
        }
        
        public void CreatingS3Bucket( )
        {
            _status = "Creating S3 bucket for node...";
        }

        public void RunningBackupScript( )
        {
            _status = "Running AWS Backup script - Please wait, this can take a while...";
        }
    }
}