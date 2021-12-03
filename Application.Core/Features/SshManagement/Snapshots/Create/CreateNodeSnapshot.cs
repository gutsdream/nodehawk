using System;
using Application.Core.Interfaces;
using Application.Core.Models.Requests;
using Application.Core.Persistence;
using Application.Core.Extensions;
using Application.Core.JobManagement;
using Application.Core.Shared;
using Domain.Entities;
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
            ActiveJobManager activeJobManager,
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
            
            UsingJobs( activeJobManager, repository );

            OnSuccessfulValidation( async x =>
            {
                var node = await repository.Nodes
                    .Include( n => n.ConnectionDetails )
                    .Include( n => n.Snapshots )
                    .FirstAsync( n => n.Id == x.NodeId );

                var activity = new Models.ActiveJobs.CreateNodeSnapshot( node );
                RegisterActiveJob( activity );

                ConnectToNode( nodeHawkSshClient, activity, node );

                int spaceUsed = GetSpaceUsed( nodeHawkSshClient, activity );
                bool containerRunning = IsContainerRunning( nodeHawkSshClient, activity );

                node.CreateSnapshot( spaceUsed, containerRunning );

                await repository.SaveChangesAsync( );
            } );
        }

        private static void ConnectToNode( INodeHawkSshClient nodeHawkSshClient, Models.ActiveJobs.CreateNodeSnapshot activity, Node node )
        {
            activity.ConnectingToNode( );
            nodeHawkSshClient.ConnectToNode( node );
        }
        
        private static int GetSpaceUsed( INodeHawkSshClient nodeHawkSshClient, Models.ActiveJobs.CreateNodeSnapshot activity )
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
        
        private static bool IsContainerRunning( INodeHawkSshClient nodeHawkSshClient, Models.ActiveJobs.CreateNodeSnapshot activity )
        {
            activity.CheckingIfNodeOnline( );

            var containerRunningResult = nodeHawkSshClient.Run( new SshMessage("docker container inspect -f '{{.State.Running}}' otnode") );
            var containerRunning = containerRunningResult.Content.Contains( "true" );
            return containerRunning;
        }
    }
}