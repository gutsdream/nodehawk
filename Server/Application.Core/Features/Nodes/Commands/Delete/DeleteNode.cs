using System;
using Application.Core.Extensions;
using Application.Core.Models.Requests;
using Application.Core.Persistence;
using Application.Core.Shared;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Features.Nodes.Commands.Delete
{
    public class DeleteNode
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

    public class DeleteNodeHandler : ValidatableCommandHandler<DeleteNode.Command, DeleteNode.Command.Validator>
    {
        public DeleteNodeHandler( DataContext repository )
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
                    .FirstAsync( n => n.Id == x.NodeId );
                repository.Remove( node.ConnectionDetails );
                repository.Remove( node );

                await repository.SaveChangesAsync( );
            } );
        }
    }
}