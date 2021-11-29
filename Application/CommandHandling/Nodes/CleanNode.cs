using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using Application.CommandHandling.Nodes.Snapshots;
using Application.Constants;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.JobActivities;
using Application.Models.Requests;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace Application.CommandHandling.Nodes
{
    public class CleanNode
    {
        public class Command : ValidatableCommand<Command, Command.Validator>
        {
            public Guid NodeId { get; set; }

            public class Validator : AbstractValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.NodeId ).NotEqual( ( Guid ) default );
                }
            }
        }
    }

    // TODO: Validator should be a part of the command, why do we need to specify the validator here? seems redundant 
    public class CleanNodeCommandHandler : ValidatableCommandHandler<CleanNode.Command, CleanNode.Command.Validator>
    {
        public CleanNodeCommandHandler( IRepository repository,
            INodeHawkSshClient sshClient,
            JobActivityManager jobActivityManager,
            IBackgroundTaskManager backgroundTaskManager )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( !await repository.Exists<Node>( n => n.Id == x.NodeId ) )
                {
                    result.AddError( nameof( x.NodeId ), $"A node with {nameof( x.NodeId )} '{x.NodeId}' was not found." );
                }

                return result;
            } );

            // Thank u otnode.com <3
            OnSuccessfulValidation( async x =>
            {
                var node = await repository.Get<Node>( )
                    .Include( n => n.ConnectionDetails )
                    .FirstAsync( n => n.Id == x.NodeId );

                var cleanNodeActivity = new CleanNodeActivity( node );
                jobActivityManager.RegisterActivity( cleanNodeActivity );

                ConnectToNode( sshClient, cleanNodeActivity, node );

                DeleteDockerOtNodeFile( sshClient, cleanNodeActivity );

                DeleteDockerTextLogs( sshClient, cleanNodeActivity );

                CleanCacheAndJournals( sshClient, cleanNodeActivity );

                jobActivityManager.CompleteActivity( cleanNodeActivity );

                backgroundTaskManager.QueueRequest<CreateNodeSnapshot.Command, ICommandResult>( new CreateNodeSnapshot.Command { NodeId = node.Id } );
            } );
        }

        private static void CleanCacheAndJournals( INodeHawkSshClient sshClient, CleanNodeActivity cleanNodeActivity )
        {
            cleanNodeActivity.CleaningCacheAndJournals( );
            sshClient.Run( Ssh.Commands.SpaceManagement.CleanCacheAndJournals );
        }

        private static void DeleteDockerTextLogs( INodeHawkSshClient sshClient, CleanNodeActivity cleanNodeActivity )
        {
            cleanNodeActivity.DeletingDockerTextLogs( );
            sshClient.Run( Ssh.Commands.SpaceManagement.DeleteAllDockerTextLogs );
        }

        private static void DeleteDockerOtNodeFile( INodeHawkSshClient sshClient, CleanNodeActivity cleanNodeActivity )
        {
            cleanNodeActivity.DeletingDockerOtNodeFile( );
            sshClient.Run( Ssh.Commands.SpaceManagement.DeleteDockerOtNodeLogFile );
        }

        private static void ConnectToNode( INodeHawkSshClient sshClient, CleanNodeActivity cleanNodeActivity, Node node )
        {
            cleanNodeActivity.ConnectingToNode( );
            sshClient.ConnectToNode( node );
        }
    }
}