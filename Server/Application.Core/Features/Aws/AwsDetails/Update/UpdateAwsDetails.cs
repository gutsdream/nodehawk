using Application.Core.Features.Aws.AwsDetails.Register;
using Application.Core.Interfaces;
using Application.Core.Models.Requests;
using Application.Core.Extensions;
using Application.Core.Persistence;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Application.Core.Shared;
using Domain.ValueObjects;

namespace Application.Core.Features.Aws.AwsDetails.Update
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

    public class UpdateAwsDetailsHandler : ValidatableCommandHandler<UpdateAwsDetails.Command, UpdateAwsDetails.Command.Validator>
    {
        public UpdateAwsDetailsHandler( DataContext repository, ICypherService cypherService )
        {
            Validate( async x =>
            {
                var result = new ValidationResult( );

                if ( !await repository.AwsDetails.AnyAsync( ) )
                {
                    result.AddError( nameof( Domain.Entities.AwsDetails ), "No AWS keys exist to update." );
                }

                return result;
            } );

            OnSuccessfulValidation( async x =>
            {
                var details = await repository.AwsDetails.FirstAsync( );

                details.UpdateAccessKey( new NotNullOrWhitespace( cypherService.Encrypt( x.AccessKey ) ) );
                details.UpdateSecretKey( new NotNullOrWhitespace( cypherService.Encrypt( x.SecretKey ) ) );

                await repository.SaveChangesAsync( );
            } );
        }
    }
}