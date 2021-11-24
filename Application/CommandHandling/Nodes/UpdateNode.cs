using System;
using Application.CommandHandling.Nodes.Interfaces;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Validators.Nodes;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace Application.CommandHandling.Nodes
{
    public class UpdateNode
    {
        public class Command : ValidatableCommand<Command, Command.Validator>, IMutateNode
        {
            public Guid NodeId { get; set; }
            public string Title { get; set; }
            public string ExternalId { get; set; }

            public string Host { get; set; }
            public string Username { get; set; }
            public string Key { get; set; }

            public class Validator : MutateNodeValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.NodeId ).NotNull( );
                    RuleFor( x => x.NodeId ).NotEqual( Guid.Empty );
                }
            }
        }
    }

    public class UpdateNodeHandler : ValidatableCommandHandler<UpdateNode.Command, UpdateNode.Command.Validator>
    {
        public UpdateNodeHandler( IRepository repository, ICypherService cypherService )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( !await repository.Exists<Node>( n => n.Id == x.NodeId ) )
                {
                    result.AddError( nameof( x.NodeId ), $"A node with {nameof( Node.Id )} '{x.NodeId}' was not found." );
                }

                if ( await repository.Exists<Node>( n => n.Title == x.Title && n.Id != x.NodeId ) )
                {
                    result.AddError( nameof( x.Title ), $"A different node with {nameof( Node.Title )} '{x.Title}' already exists." );
                }

                if ( x.ExternalId != null && await repository.Exists<Node>( n => n.ExternalId == x.ExternalId && n.Id != x.NodeId ) )
                {
                    result.AddError( nameof( x.ExternalId ), $"A different node with {nameof( Node.ExternalId )} '{x.ExternalId}' already exists." );
                }

                return result;
            } );

            OnSuccess( async x =>
            {
                var node = await repository.Get<Node>( ).FirstOrDefaultAsync( n => n.Id == x.NodeId );
                
                var connectionDetails = new ConnectionDetails( cypherService.Encrypt( x.Host ),
                    cypherService.Encrypt( x.Username ),
                    cypherService.Encrypt( x.Key ) );

                node.SetConnectionDetails( connectionDetails );
                node.SetTitle( x.Title );
                node.SetExternalId( x.ExternalId );

                await repository.SaveAsync( );
            } );
        }
    }
}