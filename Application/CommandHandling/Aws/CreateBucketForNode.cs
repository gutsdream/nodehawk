using System;
using System.Linq;
using System.Threading;
using Application.CommandHandling.Aws.Helpers;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.JobActivities;
using Application.Models.Requests;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.CommandHandling.Aws
{
    public class CreateBucketForNode
    {
        public class Command : ValidatableCommand<Command, Command.Validator>
        {
            public Guid NodeId { get; set; }

            public class Validator : AbstractValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.NodeId ).NotEmpty( );
                }
            }
        }
    }

    public class CreateBucketForNodeHandler : ValidatableCommandHandler<CreateBucketForNode.Command, CreateBucketForNode.Command.Validator>
    {
        public CreateBucketForNodeHandler( DataContext repository, ICypherService cypherService, JobActivityManager jobActivityManager )
        {
            Validate( async x =>
            {
                // TODO: not sure if the job acticvity manager is working here for whatever reason?
                var result = new ValidationResult( );

                var node = await repository.Nodes.FirstOrDefaultAsync( n => n.Id == x.NodeId );

                if ( node == default )
                {
                    result.AddError( nameof( x.NodeId ), $"Node with {nameof( Node.Id )} '{x.NodeId}' does not exist." );
                }

                var awsDetails = await repository.AwsDetails.FirstOrDefaultAsync( );
                if ( awsDetails == default )
                {
                    result.AddError( nameof( AwsDetails ), $"AWS Details not found." );
                }

                var client = AwsClientFactory.GetS3ClientFromAwsDetails( cypherService, awsDetails );
                var buckets = await client.ListBucketsAsync( CancellationToken.None );
                if ( buckets.Buckets.Any( b => b.BucketName == AwsNameResolver.BucketNameForNode( node ) ) )
                {
                    result.AddError( nameof( x.NodeId ), $"Bucket for {nameof( Node )} '{x.NodeId}' already exists." );
                }

                return result;
            } );

            OnSuccessfulValidation( async x =>
            {
                var node = await repository.Nodes.FirstAsync( n => n.Id == x.NodeId );

                var bucketCreationActivity = new BucketCreationActivity( node );
                jobActivityManager.RegisterActivity( bucketCreationActivity );

                var awsDetails = await repository.AwsDetails.FirstAsync( );
                var client = AwsClientFactory.GetS3ClientFromAwsDetails( cypherService, awsDetails );

                bucketCreationActivity.CreatingBucket( );
                await client.PutBucketAsync( AwsNameResolver.BucketNameForNode( node ) );

                jobActivityManager.CompleteActivity( bucketCreationActivity );
            } );
        }
    }
}