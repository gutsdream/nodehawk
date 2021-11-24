using System;
using Application.Extensions;
using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Application.Nodes.Commands.Interfaces;
using Application.Validators.Nodes;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Nodes.Commands
{
    public class UpdateNode
    {
        public class Request : ValidatableRequest<Request, Request.Validator>, IMutateNode
        {
            public Guid NodeId { get; set; }
            public string Title { get; set; }
            public string ExternalId { get; set; }

            public string Host { get; set; }
            public string Username { get; set; }
            public string Key { get; set; }

            public class Validator : MutateNodeValidator<Request>
            {
                public Validator( )
                {
                    RuleFor( x => x.NodeId ).NotNull( );
                    RuleFor( x => x.NodeId ).NotEqual( Guid.Empty );
                }
            }
        }
    }

    public class UpdateNodeHandler : ValidatableRequestHandler<UpdateNode.Request, UpdateNode.Request.Validator>
    {
        public UpdateNodeHandler( IRepository repository )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( !await repository.Exists<Node>( n => n.Id == x.NodeId ) )
                {
                    result.Errors.Add( new ValidationFailure( nameof( x.NodeId ),
                        $"No node with {nameof( Node.Id )} of '{x.Title}' was found." ) );
                }

                if ( await repository.Exists<Node>( n => n.Title == x.Title ) )
                {
                    result.Errors.Add( new ValidationFailure( nameof( x.Title ),
                        $"Node with {nameof( Node.Title )} '{x.Title}' already exists." ) );
                }

                if ( x.ExternalId != null &&
                     await repository.Exists<Node>( n => n.ExternalId == x.ExternalId ) )
                {
                    result.Errors.Add( new ValidationFailure( nameof( x.ExternalId ),
                        $"Node with {nameof( Node.ExternalId )} '{x.ExternalId}' already exists." ) );
                }

                return result;
            } );

            OnSuccess( async x =>
            {
                var node = await repository.Get<Node>( ).FirstOrDefaultAsync( n => n.Id == x.NodeId );
                var connectionDetails = new ConnectionDetails( x.Host, x.Username, x.Key );

                node.SetConnectionDetails( connectionDetails );
                node.SetTitle( x.Title );
                node.SetExternalId( x.ExternalId );

                await repository.Save( );
            } );
        }
    }
}