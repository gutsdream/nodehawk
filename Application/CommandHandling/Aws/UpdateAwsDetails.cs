using System.Linq;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace Application.CommandHandling.Aws
{
    public class UpdateAwsDetails
    {
        public class Command : ValidatableCommand<Command, Command.Validator>
        {
            public string AccessKey { get; set; }
            public string SecretKey { get; set; }

            public class Validator : AbstractValidator<Command>
            {
                public Validator( )
                {
                    RuleFor( x => x.AccessKey ).NotEmpty( );
                    RuleFor( x => x.SecretKey ).NotEmpty( );
                }
            }
        }
    }

    public class UpdateAwsDetailsHandler : ValidatableCommandHandler<RegisterAwsDetails.Command, RegisterAwsDetails.Command.Validator>
    {
        public UpdateAwsDetailsHandler( IRepository repository, ICypherService cypherService )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                var existingDetails = await repository.Get<AwsDetails>( ).ToListAsync( );
                if ( !existingDetails.Any( ) )
                {
                    result.AddError( nameof( AwsDetails ), "No AWS keys exist to update." );
                }

                return result;
            } );

            OnSuccessfulValidation( async x =>
            {
                var details = await repository.Get<AwsDetails>( ).FirstAsync( );
                
                details.UpdateAccessKey( cypherService.Encrypt( x.AccessKey ) );
                details.UpdateSecretKey( cypherService.Encrypt( x.SecretKey ) );
                
                repository.Add( details );
                await repository.SaveAsync( );
            } );
        }
    }
}