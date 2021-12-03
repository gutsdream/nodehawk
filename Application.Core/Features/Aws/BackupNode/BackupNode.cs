using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Application.Core.Interfaces;
using Application.Core.Models.Requests;
using Application.Core.Extensions;
using Application.Core.JobManagement;
using Application.Core.Persistence;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Application.Core.Shared;
using Application.Core.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Aws.BackupNode
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
    // TODO: protect against multiple requests

    public class BackupNodeHandler : ValidatableCommandHandler<BackupNode.Command, BackupNode.Command.Validator>
    {
        public BackupNodeHandler( DataContext repository,
            ICypherService cypherService,
            ActiveJobManager activeJobManager,
            INodeHawkSshClient sshClient,
            IEventManager eventManager )
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
                    result.AddError( nameof( Domain.Entities.AwsDetails ), $"AWS Details not found." );
                }

                return result;
            } );

            UsingJobs( activeJobManager, repository );

            OnSuccessfulValidation( async x =>
            {
                var node = await repository.Nodes
                    .Include( n => n.ConnectionDetails )
                    .FirstAsync( n => n.Id == x.NodeId );

                var awsDetails = await repository.AwsDetails.FirstOrDefaultAsync( );

                var nodeBackupActivity = new Models.ActiveJobs.BackupNode( node );
                RegisterActiveJob( nodeBackupActivity );

                var client = GetS3ClientFromAwsDetails( cypherService, awsDetails );

                await CreateS3BucketForNodeIfNotFound( client, node, nodeBackupActivity );

                BackupNode( cypherService, sshClient, awsDetails, node, nodeBackupActivity );

                node.AuditBackup( );
                await repository.SaveChangesAsync( );

                eventManager.PublishEvent( new NodeBackedUpEvent( node.Id ) );
            } );
        }

        private static string GetBucketNameForNode( Node node )
        {
            return $"otnode{node.Id}";
        }

        private static AmazonS3Client GetS3ClientFromAwsDetails( ICypherService cypherService, Domain.Entities.AwsDetails awsDetails )
        {
            var credentials = new BasicAWSCredentials( cypherService.Decrypt( awsDetails.AccessKey ),
                cypherService.Decrypt( awsDetails.SecretKey ) );

            // TODO: Maybe add a preferred region to AWS Details that can pop thru here
            return new AmazonS3Client( credentials, RegionEndpoint.USWest1 );
        }

        private static async Task CreateS3BucketForNodeIfNotFound( AmazonS3Client client,
            Node node,
            Models.ActiveJobs.BackupNode nodeBackup )
        {
            var buckets = await client.ListBucketsAsync( CancellationToken.None );
            if ( buckets.Buckets.All( b => b.BucketName != GetBucketNameForNode( node ) ) )
            {
                nodeBackup.CreatingS3Bucket( );
                await client.PutBucketAsync( GetBucketNameForNode( node ) );
            }
        }

        private static void BackupNode( ICypherService cypherService,
            INodeHawkSshClient sshClient,
            Domain.Entities.AwsDetails awsDetails,
            Node node,
            Models.ActiveJobs.BackupNode nodeBackup )
        {
            nodeBackup.ConnectingToNode( );
            sshClient.ConnectToNode( node );

            nodeBackup.RunningBackupScript( );
            sshClient.Run( BackupScriptSshCommand( cypherService.Decrypt( awsDetails.AccessKey ),
                cypherService.Decrypt( awsDetails.SecretKey ),
                GetBucketNameForNode( node ) ) );
        }

        private static SshMessage BackupScriptSshCommand( string accessKey, string secretKey, string bucketName )
        {
            return new SshMessage( $"docker exec otnode node scripts/backup-upload-aws.js --config=/ot-node/.origintrail_noderc " +
                                   $"--configDir=/ot-node/data --backupDirectory=/ot-node/backup " +
                                   $"--AWSAccessKeyId={accessKey} --AWSSecretAccessKey={secretKey} --AWSBucketName={bucketName}" );
        }
    }

    public class NodeBackedUpEvent : IApplicationEvent
    {
        public Guid NodeId { get; }

        public NodeBackedUpEvent( Guid nodeId )
        {
            NodeId = nodeId;
        }
    }
}