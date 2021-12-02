using System;
using Application.Core.Extensions;
using Application.Core.Interfaces;
using Application.Core.Models.Requests;
using Application.Core.Persistence;
using Application.Core.Shared;
using Application.Core.Shared.Interfaces;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Nodes.Commands.Create
{
    public class CreateNode
    {
        public class Command : ValidatableCommand<Command, Command.Validator>
        {
            public string Title { get; set; }
            public string ExternalId { get; set; }

            public string Host { get; set; }
            public string Username { get; set; }
            public string Key { get; set; }

            public class Validator : AbstractValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.Title ).NotEmpty( );

                    // Should either be null or properly formed
                    RuleFor( x => x.ExternalId ).Length( 40 ).When( x => x.ExternalId != null );

                    // Ensure SSH connection details provided
                    RuleFor( x => x.Host ).NotEmpty( );
                    RuleFor( x => x.Username ).NotEmpty( );
                    RuleFor( x => x.Key ).NotEmpty( );
                }
            }
        }
    }

    public class CreateNodeHandler : ValidatableCommandHandler<CreateNode.Command, CreateNode.Command.Validator>
    {
        public CreateNodeHandler( DataContext repository, ICypherService cypherService, IEventManager eventManager )
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
                
                // TODO: replace with agnostic event, remove coupling to snapshot slice
                eventManager.PublishEvent( new NodeCreatedEvent( node.Id ) );
            } );
        }
    }
    
    public class NodeCreatedEvent : IApplicationEvent
    {
        public Guid NodeId { get; }

        public NodeCreatedEvent( Guid nodeId )
        {
            NodeId = nodeId;
        }
    }
}