using Application.Extensions;
using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Application.Nodes.Commands.Interfaces;
using Application.Validators.Nodes;
using Domain.Entities;
using FluentValidation.Results;

namespace Application.Nodes.Commands
{
    public class CreateNode
    {
        public class Request : ValidatableRequest<Request, MutateNodeValidator<Request>>, IMutateNode
        {
            public string Title { get; set; }
            public string ExternalId { get; set; }

            public string Host { get; set; }
            public string Username { get; set; }
            public string Key { get; set; }
        }
    }

    public class CreateNodeHandler : ValidatableRequestHandler<CreateNode.Request, MutateNodeValidator<CreateNode.Request>>
    {
        public CreateNodeHandler( IRepository repository )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( await repository.Exists<Node>( n => n.Title == x.Title ) )
                {
                    result.Errors.Add( new ValidationFailure( nameof( x.Title ),
                        $"Node with {nameof( Node.Title )} '{x.Title}' already exists" ) );
                }

                if ( x.ExternalId != null &&
                     await repository.Exists<Node>( n => n.ExternalId == x.ExternalId ) )
                {
                    result.Errors.Add( new ValidationFailure( nameof( x.ExternalId ),
                        $"Node with {nameof( Node.ExternalId )} '{x.ExternalId}' already exists" ) );
                }

                return result;
            } );

            OnSuccess( async x =>
            {
                var connectionDetails = new ConnectionDetails( x.Host, x.Username, x.Key );

                var node = new Node( x.Title, connectionDetails, x.ExternalId );
                await repository.Add( node );

                //TODO: use a command post processor, remove Save from IRepository interface 
                await repository.Save( );
            } );
        }
    }
}