using System;
using Application.Constants;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.JobActivities;
using Application.Models.Requests;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.CommandHandling.Snapshots
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
        public CreateNodeSnapshotHandler( DataContext repository, INodeHawkSshClient nodeHawkSshClient, JobActivityManager jobActivityManager )
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

                var activity = new CreateNodeSnapshotActivity( node );
                jobActivityManager.RegisterActivity( activity );

                ConnectToNode( nodeHawkSshClient, activity, node );

                int spaceUsed = GetSpaceUsed( nodeHawkSshClient, activity );
                bool containerRunning = IsContainerRunning( nodeHawkSshClient, activity );

                node.CreateSnapshot( spaceUsed, containerRunning );
                jobActivityManager.CompleteActivity( activity );

                await repository.SaveChangesAsync( );
            } );
        }

        private static void ConnectToNode( INodeHawkSshClient nodeHawkSshClient, CreateNodeSnapshotActivity activity, Node node )
        {
            activity.ConnectingToNode( );
            nodeHawkSshClient.ConnectToNode( node );
        }

        private static int GetSpaceUsed( INodeHawkSshClient nodeHawkSshClient, CreateNodeSnapshotActivity activity )
        {
            activity.CheckingSpaceUsed( );

            var dfCommandResult = nodeHawkSshClient.Run( Ssh.Queries.GetDiskSpace );
            var spaceUsed = GetSpaceUsedPercentageFromSshResult( dfCommandResult );

            return spaceUsed;
        }
        
        private static bool IsContainerRunning( INodeHawkSshClient nodeHawkSshClient, CreateNodeSnapshotActivity activity )
        {
            activity.CheckingIfNodeOnline( );

            var containerRunningResult = nodeHawkSshClient.Run( Ssh.Queries.IsContainerRunning );
            var containerRunning = containerRunningResult.Content.Contains( "true" );
            return containerRunning;
        }

        /// <summary>
        /// Parses the response of a CLI 'df .h' command
        /// </summary>
        private static int GetSpaceUsedPercentageFromSshResult( ISshCommandResult result )
        {
            return result.Content
                .SplitToList( )
                // Remove whitespace and line breaks
                .Ignore( string.Empty, " ", "/\n" )
                .FindWord( "Mounted" )
                .SkipWords( 5 )
                .Get( )
                .RemoveNonNumericCharacters( )
                .AsInt( );
        }
    }
}