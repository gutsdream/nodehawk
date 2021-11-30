using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Application.CommandHandling.Aws.Helpers;
using Application.Constants;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.JobActivities;
using Application.Models.Requests;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.CommandHandling.Aws
{
    public class BackupNode
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

    public class BackupNodeHandler : ValidatableCommandHandler<BackupNode.Command, BackupNode.Command.Validator>
    {
        public BackupNodeHandler( DataContext repository,
            ICypherService cypherService,
            JobActivityManager jobActivityManager,
            INodeHawkSshClient sshClient,
            IMediator mediator )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                var node = await repository.Nodes.FirstOrDefaultAsync( n => n.Id == x.NodeId );

                if ( node == default )
                {
                    result.AddError( nameof( x.NodeId ), $"Node with {nameof( Node.Id )} '{x.NodeId}' does not exist." );
                }

                var awsDetails = await repository.Nodes.FirstOrDefaultAsync( );
                if ( awsDetails == default )
                {
                    result.AddError( nameof( AwsDetails ), $"AWS Details not found." );
                }

                return result;
            } );

            OnSuccessfulValidation( async x =>
            {
                var node = await repository.Nodes
                    .Include( n => n.ConnectionDetails )
                    .FirstAsync( n => n.Id == x.NodeId );

                var awsDetails = await repository.AwsDetails.FirstOrDefaultAsync( );

                var nodeBackupActivity = new BackupNodeActivity( node );
                jobActivityManager.RegisterActivity( nodeBackupActivity );

                var client = AwsClientFactory.GetS3ClientFromAwsDetails( cypherService, awsDetails );
                await CreateS3BucketForNodeIfNotFound( mediator, client, node, x );

                BackupNode( cypherService, sshClient, awsDetails, node, nodeBackupActivity );

                jobActivityManager.CompleteActivity( nodeBackupActivity );
            } );
        }

        private static void BackupNode( ICypherService cypherService,
            INodeHawkSshClient sshClient, 
            AwsDetails awsDetails, 
            Node node,
            BackupNodeActivity nodeBackupActivity )
        {
            nodeBackupActivity.ConnectingToNode( );
            sshClient.ConnectToNode( node );
            
            var nodeCommand = Ssh.Commands.Aws.RunBackupScript( cypherService.Decrypt( awsDetails.AccessKey ),
                cypherService.Decrypt( awsDetails.SecretKey ),
                AwsNameResolver.BucketNameForNode( node ) );

            nodeBackupActivity.RunningBackupScript( );
            sshClient.Run( nodeCommand );
        }

        private static async Task CreateS3BucketForNodeIfNotFound( IMediator mediator, AmazonS3Client client, Node node, BackupNode.Command x )
        {
            var buckets = await client.ListBucketsAsync( CancellationToken.None );
            // maybe move to OnSuccess?? should work desu
            if ( buckets.Buckets.All( b => b.BucketName != AwsNameResolver.BucketNameForNode( node ) ) )
            {
                // Create bucket if doesn't exist
                await mediator.Send( new CreateBucketForNode.Command { NodeId = x.NodeId } );
            }
        }
    }
}