using Application.CommandHandling.Nodes.Interfaces;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Validators.Nodes;
using Domain.Entities;
using FluentValidation.Results;

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
        public CreateNodeHandler( IRepository repository, ICypherService cypherService )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( await repository.Exists<Node>( n => n.Title == x.Title ) )
                {
                    result.AddError( nameof( x.Title ), $"Node with {nameof( Node.Title )} '{x.Title}' already exists" );
                }

                if ( x.ExternalId != null && await repository.Exists<Node>( n => n.ExternalId == x.ExternalId ) )
                {
                    result.AddError( nameof( x.ExternalId ), $"Node with {nameof( Node.ExternalId )} '{x.ExternalId}' already exists" );
                }

                return result;
            } );

            OnSuccess( async x =>
            {
                var connectionDetails = new ConnectionDetails( cypherService.Encrypt( x.Host ),
                    cypherService.Encrypt( x.Username ),
                    cypherService.Encrypt( x.Key ) );

                var node = new Node( x.Title, connectionDetails, x.ExternalId );
                await repository.AddAsync( node );

                //TODO: use a command post processor, remove Save from IRepository interface 
                await repository.SaveAsync( );
            } );
        }
    }
}