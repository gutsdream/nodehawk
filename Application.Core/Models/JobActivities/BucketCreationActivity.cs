using System;
using Application.Core.JobState;
using Domain.Entities;

namespace Application.Core.Models.JobActivities
{
    public class BucketCreationActivity : IJobActivity
    {
        public Guid Id { get; }
        public string Title => $"Creating S3 bucket for Node: '{_nodeTitle}'";
        public string Status => _status;
        
        private readonly string _nodeTitle;
        private string _status;

        public BucketCreationActivity( Node node )
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