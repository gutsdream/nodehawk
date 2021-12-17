using System;
using Application.Core.Interfaces;
using Application.Core.Models.Requests;
using Application.Core.Persistence;
using Application.Core.Extensions;
using Application.Core.JobManagement;
using Application.Core.Shared;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.SshManagement.Snapshots.Create
{
    public class CreateNodeSnapshot
    {
        public class Command : ValidatableCommand<Command, Command.Validator>
        {
            public Guid NodeId { get; set; }

            public class Validator : AbstractValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.NodeId ).NotNull( );
                    RuleFor( x => x.NodeId ).NotEqual( Guid.Empty );
                }
            }
        }
    }

    public class CreateNodeSnapshotHandler : ValidatableCommandHandler<CreateNodeSnapshot.Command, CreateNodeSnapshot.Command.Validator>
    {
        public CreateNodeSnapshotHandler( DataContext repository, 
            INodeHawkSshClient nodeHawkSshClient, 
            TransientJobManagerFactory transientJobManagerFactory )
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
            
            OnSuccessfulValidation( async x =>
            {
                var node = await repository.Nodes
                    .Include( n => n.ConnectionDetails )
                    .Include( n => n.Snapshots )
                    .FirstAsync( n => n.Id == x.NodeId );

                var activity = new CreateNodeSnapshotJob( node );
                
                using var transientJobManager = transientJobManagerFactory.Create( );
                transientJobManager.RegisterActiveJob( activity );

                ConnectToNode( nodeHawkSshClient, activity, node );

                int spaceUsed = GetSpaceUsedOnDrive( nodeHawkSshClient, activity );
                bool containerRunning = IsContainerRunning( nodeHawkSshClient, activity );

                node.CreateSnapshot( new Percentage( spaceUsed ), containerRunning );

                await repository.SaveChangesAsync( );
                transientJobManager.MarkJobAsSuccess( activity );
            } );
        }

        private static void ConnectToNode( INodeHawkSshClient nodeHawkSshClient, CreateNodeSnapshotJob activity, Node node )
        {
            activity.ConnectingToNode( );
            nodeHawkSshClient.ConnectToNode( node );
        }
        
        private static int GetSpaceUsedOnDrive( INodeHawkSshClient nodeHawkSshClient, CreateNodeSnapshotJob activity )
        {
            activity.CheckingSpaceUsed( );

            var dfCommandResult = nodeHawkSshClient.Run( new SshMessage("df .") );
            var spaceUsed = dfCommandResult.Content
                .SplitToList( )
                // Remove whitespace and line breaks
                .Ignore( string.Empty, " ", "/\n" )
                .FindWord( "Mounted" )
                .SkipWords( 5 )
                .Get( )
                .RemoveNonNumericCharacters( )
                .AsInt( );

            return spaceUsed;
        }
        
        private static bool IsContainerRunning( INodeHawkSshClient nodeHawkSshClient, CreateNodeSnapshotJob activity )
        {
            activity.CheckingIfNodeOnline( );

            var containerRunningResult = nodeHawkSshClient.Run( new SshMessage("docker container inspect -f '{{.State.Running}}' otnode") );
            var containerRunning = containerRunningResult.Content.Contains( "true" );
            return containerRunning;
        }
    }
}