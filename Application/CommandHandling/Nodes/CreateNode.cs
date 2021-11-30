using Application.CommandHandling.Nodes.Interfaces;
using Application.CommandHandling.Snapshots;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Validators.Nodes;
using Domain.Entities;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.CommandHandling.Nodes
{
    public class CreateNode
    {
        public class Command : ValidatableCommand<Command, MutateNodeValidator<Command>>, IMutateNode
        {
            public string Title { get; set; }
            public string ExternalId { get; set; }

            public string Host { get; set; }
            public string Username { get; set; }
            public string Key { get; set; }
        }
    }

    public class CreateNodeHandler : ValidatableCommandHandler<CreateNode.Command, MutateNodeValidator<CreateNode.Command>>
    {
        public CreateNodeHandler( DataContext repository, ICypherService cypherService, IBackgroundTaskManager backgroundTaskManager )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( await repository.Nodes.AnyAsync( n => n.Title == x.Title ) )
                {
                    result.AddError( nameof( x.Title ), $"Node with {nameof( Node.Title )} '{x.Title}' already exists." );
                }

                if ( x.ExternalId != null && await repository.Nodes.AnyAsync( n => n.ExternalId == x.ExternalId ) )
                {
                    result.AddError( nameof( x.ExternalId ), $"Node with {nameof( Node.ExternalId )} '{x.ExternalId}' already exists." );
                }

                return result;
            } );

            OnSuccessfulValidation( async x =>
            {
                var connectionDetails = new ConnectionDetails( cypherService.Encrypt( x.Host ),
                    cypherService.Encrypt( x.Username ),
                    cypherService.Encrypt( x.Key ) );

                var node = new Node( x.Title, connectionDetails, x.ExternalId );
                repository.Add( node );
                
                //TODO: use a command post processor, remove Save from IRepository interface 
                await repository.SaveChangesAsync( );
                
                // TODO: Move into SaveAsync eventhandler thing
                backgroundTaskManager.QueueRequest<CreateNodeSnapshot.Command, ICommandResult>( new CreateNodeSnapshot.Command{ NodeId = node.Id } );
            } );
        }
    }
}