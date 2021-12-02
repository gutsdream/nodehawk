using System;
using System.Collections.Generic;
using Application.Core.Extensions;
using Application.Core.Interfaces;
using Application.Core.JobState;
using Application.Core.Models.JobActivities;
using Application.Core.Models.Requests;
using Application.Core.Persistence;
using Application.Core.Shared;
using Application.Core.Shared.Interfaces;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.SshManagement.SpaceManagement.Clean
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
                    RuleFor( x => x.NodeId ).NotEmpty( );
                }
            }
        }
    }

    public class CleanNodeCommandHandler : ValidatableCommandHandler<CleanNode.Command, CleanNode.Command.Validator>
    {
        public CleanNodeCommandHandler( DataContext repository,
            INodeHawkSshClient sshClient,
            JobActivityManager jobActivityManager,
            IEventManager eventManager )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( !await repository.Nodes.AnyAsync( n => n.Id == x.NodeId ) )
                {
                    result.AddError( nameof( x.NodeId ), $"A node with {nameof( x.NodeId )} '{x.NodeId}' was not found." );
                }

                return result;
            } );

            // Thank u otnode.com <3
            OnSuccessfulValidation( async x =>
            {
                var node = await repository.Nodes
                    .Include( n => n.ConnectionDetails )
                    .FirstAsync( n => n.Id == x.NodeId );

                var cleanNodeActivity = new CleanNodeActivity( node );
                jobActivityManager.RegisterActivity( cleanNodeActivity );

                ConnectToNode( sshClient, cleanNodeActivity, node );

                DeleteDockerOtNodeLogFile( sshClient, cleanNodeActivity );

                DeleteDockerTextLogs( sshClient, cleanNodeActivity );

                CleanCacheAndJournals( sshClient, cleanNodeActivity );

                jobActivityManager.CompleteActivity( cleanNodeActivity );

                eventManager.PublishEvent( new NodeCleanedEvent( node.Id ) );
            } );
        }

        private static void ConnectToNode( INodeHawkSshClient sshClient, CleanNodeActivity cleanNodeActivity, Node node )
        {
            cleanNodeActivity.ConnectingToNode( );
            sshClient.ConnectToNode( node );
        }

        private static void DeleteDockerOtNodeLogFile( INodeHawkSshClient sshClient, CleanNodeActivity cleanNodeActivity )
        {
            cleanNodeActivity.DeletingDockerOtNodeLogFile( );
            sshClient.Run( new SshMessage( "truncate -s 0 $(docker inspect -f '{{.LogPath}}' otnode)" ) );
        }

        private static void DeleteDockerTextLogs( INodeHawkSshClient sshClient, CleanNodeActivity cleanNodeActivity )
        {
            cleanNodeActivity.DeletingDockerTextLogs( );
            sshClient.Run( new List<SshMessage>
            {
                new("cd  /var/lib/docker/overlay2"),
                new("find . -type f -name \"*.log\" -delete")
            } );
        }

        private static void CleanCacheAndJournals( INodeHawkSshClient sshClient, CleanNodeActivity cleanNodeActivity )
        {
            cleanNodeActivity.CleaningCacheAndJournals( );
            sshClient.Run( new List<SshMessage>
            {
                new("apt clean"),
                new("journalctl --vacuum-time=1h")
            } );
        }
    }

    public class NodeCleanedEvent : IApplicationEvent
    {
        public Guid NodeId { get; }

        public NodeCleanedEvent( Guid nodeId )
        {
            NodeId = nodeId;
        }
    }
}