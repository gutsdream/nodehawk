using System;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace Application.CommandHandling.Nodes.Snapshots
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
        public CreateNodeSnapshotHandler( IRepository repository, INodeHawkSshClient nodeHawkSshClient )
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

            OnSuccess( async x =>
            {
                var node = await repository.Get<Node>( )
                    .Include( x => x.ConnectionDetails )
                    .FirstAsync( n => n.Id == x.NodeId );

                nodeHawkSshClient.ConnectToNode( node );
                var dfCommandResult = nodeHawkSshClient.Run( "df ." );

                var spaceUsed = GetSpaceUsedPercentageFromSshResult( dfCommandResult );
                
                var containerRunningResult = nodeHawkSshClient.Run( "docker container inspect -f '{{.State.Running}}' otnode" );
                var containerRunning = containerRunningResult.Content.Contains( "true" );

                node.CreateSnapshot( spaceUsed, containerRunning );

                //TODO: use a command post processor, remove Save from IRepository interface 
                await repository.SaveAsync( );
            } );
        }

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