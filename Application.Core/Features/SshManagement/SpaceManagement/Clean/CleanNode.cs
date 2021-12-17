using System;
using System.Collections.Generic;
using Application.Core.Extensions;
using Application.Core.Interfaces;
using Application.Core.JobManagement;
using Application.Core.Models.Requests;
using Application.Core.Persistence;
using Application.Core.Shared;
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
            TransientJobManagerFactory transientJobManagerFactory,
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

                var cleanNodeActivity = new CleanNodeJob( node );
                
                using var transientJobManager = transientJobManagerFactory.Create( );
                transientJobManager.RegisterActiveJob( cleanNodeActivity );

                ConnectToNode( sshClient, cleanNodeActivity, node );

                DeleteDockerOtNodeLogFile( sshClient, cleanNodeActivity );

                DeleteDockerTextLogs( sshClient, cleanNodeActivity );

                CleanCacheAndJournals( sshClient, cleanNodeActivity );

                node.AuditCleanup( );
                await repository.SaveChangesAsync( );
                
                transientJobManager.MarkJobAsSuccess( cleanNodeActivity );

                eventManager.PublishEvent( new NodeCleanedEvent( node.Id ) );
            } );
        }

        private static void ConnectToNode( INodeHawkSshClient sshClient, CleanNodeJob cleanNodeJob, Node node )
        {
            cleanNodeJob.ConnectingToNode( );
            sshClient.ConnectToNode( node );
        }

        private static void DeleteDockerOtNodeLogFile( INodeHawkSshClient sshClient, CleanNodeJob cleanNodeJob )
        {
            cleanNodeJob.DeletingDockerOtNodeLogFile( );
            sshClient.Run( new SshMessage( "truncate -s 0 $(docker inspect -f '{{.LogPath}}' otnode)" ) );
        }

        private static void DeleteDockerTextLogs( INodeHawkSshClient sshClient, CleanNodeJob cleanNodeJob )
        {
            cleanNodeJob.DeletingDockerTextLogs( );
            sshClient.Run( new List<SshMessage>
            {
                new("cd  /var/lib/docker/overlay2"),
                new("find . -type f -name \"*.log\" -delete")
            } );
        }

        private static void CleanCacheAndJournals( INodeHawkSshClient sshClient, CleanNodeJob cleanNodeJob )
        {
            cleanNodeJob.CleaningCacheAndJournals( );
            sshClient.Run( new List<SshMessage>
            {
                new("apt clean"),
                new("journalctl --vacuum-time=1h")
            } );
        }
    }
}