using System;
using Application.Core.Extensions;
using Application.Core.Interfaces;
using Application.Core.Models.Requests;
using Application.Core.Persistence;
using Application.Core.Shared;
using Domain.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Generics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Nodes.Commands.Update.Details
{
    public class UpdateNodeDetails
    {
        public class Command : ValidatableCommand<Command, Command.Validator>
        {
            public Guid NodeId { get; set; }
            public string Title { get; set; }
            public string ExternalId { get; set; }

            public class Validator : AbstractValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.NodeId ).NotNull( );
                    RuleFor( x => x.NodeId ).NotEqual( Guid.Empty );

                    RuleFor( x => x.Title ).NotEmpty( );

                    // Should either be null or properly formed
                    RuleFor( x => x.ExternalId ).Length( 40 ).When( x => x.ExternalId != null );
                }
            }
        }

        public class UpdateNodeDetailsHandler : ValidatableCommandHandler<Command, Command.Validator>
        {
            public UpdateNodeDetailsHandler( DataContext repository, IEventManager eventManager )
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
                        result.AddError( nameof( x.ExternalId ),
                            $"Different node with {nameof( Node.ExternalId )} '{x.ExternalId}' already exists." );
                    }

                    return result;
                } );

                OnSuccessfulValidation( async x =>
                {
                    var node = await repository.Nodes.FirstOrDefaultAsync( n => n.Id == x.NodeId );

                    node.SetTitle( x.Title.AsNonNull( ) );
                    node.SetExternalId( new NodeExternalId( x.ExternalId ) );

                    await repository.SaveChangesAsync( );
                } );
            }
        }
    }
}