using System;
using Application.Core.Extensions;
using Application.Core.Features.SshManagement.Snapshots.Create;
using Application.Core.Interfaces;
using Application.Core.Models.Requests;
using Application.Core.Models.Results;
using Application.Core.Shared;
using Application.Core.Shared.Interfaces;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Nodes.Commands.Update
{
    public class UpdateNode
    {
        public class Command : ValidatableCommand<Command, Command.Validator>
        {
            public Guid NodeId { get; set; }
            public string Title { get; set; }
            public string ExternalId { get; set; }

            public string Host { get; set; }
            public string Username { get; set; }
            public string Key { get; set; }

            public class Validator : AbstractValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.NodeId ).NotNull( );
                    RuleFor( x => x.NodeId ).NotEqual( Guid.Empty );

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

    public class UpdateNodeHandler : ValidatableCommandHandler<UpdateNode.Command, UpdateNode.Command.Validator>
    {
        public UpdateNodeHandler( Persistence.DataContext repository, ICypherService cypherService, IEventManager eventManager )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( !await repository.Nodes.AnyAsync( n => n.Id == x.NodeId ) )
                {
                    result.AddError( nameof( x.NodeId ), $"A node with {nameof( Node.Id )} '{x.NodeId}' was not found." );
                }

                if ( await repository.Nodes.AnyAsync( n => n.Title == x.Title && n.Id != x.NodeId ) )
                {
                    result.AddError( nameof( x.Title ), $"Different node with {nameof( Node.Title )} '{x.Title}' already exists." );
                }

                if ( x.ExternalId != null && await repository.Nodes.AnyAsync( n => n.ExternalId == x.ExternalId && n.Id != x.NodeId ) )
                {
                    result.AddError( nameof( x.ExternalId ), $"Different node with {nameof( Node.ExternalId )} '{x.ExternalId}' already exists." );
                }

                return result;
            } );

            OnSuccessfulValidation( async x =>
            {
                var node = await repository.Nodes.FirstOrDefaultAsync( n => n.Id == x.NodeId );

                var connectionDetails = new ConnectionDetails( cypherService.Encrypt( x.Host ),
                    cypherService.Encrypt( x.Username ),
                    cypherService.Encrypt( x.Key ) );

                node.SetConnectionDetails( connectionDetails );
                node.SetTitle( x.Title );
                node.SetExternalId( x.ExternalId );

                await repository.SaveChangesAsync( );

                eventManager.PublishEvent( new NodeUpdatedEvent( node.Id ) );
            } );
        }
    }

    public class NodeUpdatedEvent : IApplicationEvent
    {
        public Guid NodeId { get; }

        public NodeUpdatedEvent( Guid nodeId )
        {
            NodeId = nodeId;
        }
    }
}