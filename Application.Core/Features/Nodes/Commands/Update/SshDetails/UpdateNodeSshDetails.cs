using System;
using Application.Core.Extensions;
using Application.Core.Interfaces;
using Application.Core.Models.Requests;
using Application.Core.Persistence;
using Application.Core.Shared;
using Domain.Entities;
using Domain.ValueObjects.Generics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Nodes.Commands.Update.SshDetails
{
    public class UpdateNodeSshDetails
    {
        public class Command : ValidatableCommand<Command, Command.Validator>
        {
            public Guid NodeId { get; set; }

            public string Host { get; set; }
            public string Username { get; set; }
            public string Key { get; set; }

            public class Validator : AbstractValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.NodeId ).NotNull( );
                    RuleFor( x => x.NodeId ).NotEqual( Guid.Empty );

                    // Ensure SSH connection details provided
                    RuleFor( x => x.Host ).NotEmpty( );
                    RuleFor( x => x.Username ).NotEmpty( );
                    RuleFor( x => x.Key ).NotEmpty( );
                }
            }
        }
    }

    public class UpdateNodeSshDetailsHandler : ValidatableCommandHandler<UpdateNodeSshDetails.Command, UpdateNodeSshDetails.Command.Validator>
    {
        public UpdateNodeSshDetailsHandler( DataContext repository, 
            ICypherService cypherService, 
            IEventManager eventManager,
            INodeHawkSshClient sshClient )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( !await repository.Nodes.AnyAsync( n => n.Id == x.NodeId ) )
                {
                    result.AddError( nameof( x.NodeId ), $"A node with {nameof( Node.Id )} '{x.NodeId}' was not found." );
                }
                
                if ( !sshClient.AreConnectionDetailsValid( x.Host, x.Username, x.Key ) )
                {
                    result.AddError( "SshDetails", "Could not verify authenticity of provided SSH connection details. Please ensure you have entered them correctly." );
                }

                return result;
            } );

            OnSuccessfulValidation( async x =>
            {
                var node = await repository.Nodes.FirstOrDefaultAsync( n => n.Id == x.NodeId );

                var connectionDetails = new ConnectionDetails( cypherService.Encrypt( x.Host ).AsNonNull( ),
                    cypherService.Encrypt( x.Username ).AsNonNull( ),
                    cypherService.Encrypt( x.Key ).AsNonNull( ) );

                node.SetConnectionDetails( connectionDetails.AsNonNull() );

                await repository.SaveChangesAsync( );

                eventManager.PublishEvent( new NodeSshDetailsUpdatedEvent( node.Id ) );
            } );
        }
    }
}