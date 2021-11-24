using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Models;
using Application.Models.Requests;
using Domain.ExceptionHandling;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application
{
    public abstract class ValidatableRequestHandler<TRequest, TValidator> : IRequestHandler<TRequest, ICommandResult>
        where TRequest : ValidatableRequest<TRequest, TValidator> where TValidator : AbstractValidator<TRequest>, new( )
    {
        private Func<TRequest, Task<ValidationResult>> _validation;
        private Func<TRequest, Task> _onSuccess;
        private Func<TRequest, Task<ValidationResult>> _onFailure;

        /// <summary>
        /// Optional validation to be done before any actions are carried out. 
        /// </summary>
        public void Validate( Func<TRequest, Task<ValidationResult>> validationFunc )
        {
            _validation = validationFunc;
        }

        /// <summary>
        /// Action to be done upon successful validation.
        /// </summary>
        public void OnSuccess( Func<TRequest, Task> onSuccessFunc )
        {
            _onSuccess = onSuccessFunc;
        }

        /// <summary>
        /// Optional override on failed validation.
        /// </summary>
        /// <param name="onFailureFunc"></param>
        public void OnFailure( Func<TRequest, Task<ValidationResult>> onFailureFunc )
        {
            _onFailure = onFailureFunc;
        }

        // TODO: replace ValidationResult with a custom result class
        public async Task<ICommandResult> Handle( TRequest request, CancellationToken cancellationToken )
        {
            var requestValidationResult = request.Validate( );
            if ( !requestValidationResult.IsValid )
            {
                return CommandResult( requestValidationResult );
            }

            // Handler level validation
            var handlerValidationResult = await _validation.Invoke( request );
            if ( !handlerValidationResult.IsValid )
            {
                if ( _onFailure != null )
                {
                    return CommandResult( await _onFailure.Invoke( request ) );
                }

                return CommandResult( handlerValidationResult );
            }

            Throw.IfNull( _onSuccess, nameof( _onSuccess ) );
            await _onSuccess.Invoke( request );

            // return Successful
            return CommandResult( handlerValidationResult );
        }

        private static CommandResult CommandResult( ValidationResult result )
        {
            var commandResult = new CommandResult( );

            var errors = result.Errors
                .Select( x => new Error( x.PropertyName, x.ErrorMessage ) )
                .ToList( );

            foreach ( var error in errors )
            {
                commandResult.AddError( error );
            }

            return commandResult;
        }
    }
}