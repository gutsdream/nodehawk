using Application.Extensions;
using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.CommandHandling.Aws
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
        public RegisterAwsDetailsHandler( DataContext repository, ICypherService cypherService )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                // Currently nodehawk is designed for single user operation (and single AWS acc operation, etc etc)
                // This may change in the future and if so this rule may be removed
                if ( await repository.AwsDetails.AnyAsync( ) )
                {
                    result.AddError( nameof( AwsDetails ), "Keys for AWS already exist." );
                }

                return result;
            } );

            OnSuccessfulValidation( async x =>
            {
                var details = new AwsDetails( cypherService.Encrypt( x.AccessKey ), cypherService.Encrypt( x.SecretKey ) );

                repository.Add( details );
                await repository.SaveChangesAsync( );
            } );
        }
    }
}