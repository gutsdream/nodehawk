using Application.Core.Interfaces;
using Application.Core.Models.Requests;
using Application.Core.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Application.Core.Shared;

namespace Application.Core.Features.Aws.AwsDetails.Register
{
    public class RegisterAwsDetails
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

    public class RegisterAwsDetailsHandler : ValidatableCommandHandler<RegisterAwsDetails.Command, RegisterAwsDetails.Command.Validator>
    {
        public RegisterAwsDetailsHandler( Persistence.DataContext repository, ICypherService cypherService )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                // Currently nodehawk is designed for single user operation (and single AWS acc operation, etc etc)
                // This may change in the future and if so this rule may be removed
                if ( await repository.AwsDetails.AnyAsync( ) )
                {
                    result.AddError( nameof( Domain.Entities.AwsDetails ), "Keys for AWS already exist." );
                }

                return result;
            } );

            OnSuccessfulValidation( async x =>
            {
                var details = new Domain.Entities.AwsDetails( cypherService.Encrypt( x.AccessKey ), cypherService.Encrypt( x.SecretKey ) );

                repository.Add( details );
                await repository.SaveChangesAsync( );
            } );
        }
    }
}