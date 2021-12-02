using System;
using Application.Core.JobManagement;
using Domain.Entities;

namespace Application.Core.Models.ActiveJobs
{
    public class BucketCreation : IActiveJob
    {
        public Guid Id { get; }
        public string Title => $"Creating S3 bucket for Node: '{_nodeTitle}'";
        public string Status => _status;
        
        private readonly string _nodeTitle;
        private string _status;

        public BucketCreation( Node node )
        {
            Id = Guid.NewGuid( );
            _nodeTitle = node.Title;
            _status = "Authenticating with AWS...";
        }

        public void CreatingBucket( )
        {
            _status = "Creating S3 bucket...";
        }
    }
}