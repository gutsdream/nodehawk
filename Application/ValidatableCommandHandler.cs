using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.CommandHandling;
using Application.Models.Requests;
using Domain.ExceptionHandling;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application
{
    /// <summary>
    /// Abstract base class for handling <see cref="ValidatableCommand{TCommand,TValidator}"/>.<br/><br/>
    /// Provides hooks for custom validation preconditions and success/failure hooks.
    /// </summary>
    /// <typeparam name="TCommand">Validatable command, used as an input for hooks.</typeparam>
    /// <typeparam name="TValidator">Provided validator for the command.</typeparam>
    public abstract class ValidatableCommandHandler<TCommand, TValidator> : IRequestHandler<TCommand, ICommandResult>
        where TCommand : ValidatableCommand<TCommand, TValidator> where TValidator : AbstractValidator<TCommand>, new( )
    {
        private Func<TCommand, Task<ValidationResult>> _validation;
        private Func<TCommand, Task> _onSuccess;
        private Func<TCommand, Task<ValidationResult>> _onFailure;

        /// <summary>
        /// Optional validation to be done before any actions are carried out. 
        /// </summary>
        protected void Validate( Func<TCommand, Task<ValidationResult>> validationFunc )
        {
            _validation = validationFunc;
        }

        /// <summary>
        /// Action to be done upon successful validation.
        /// </summary>
        protected void OnSuccessfulValidation( Func<TCommand, Task> onSuccessFunc )
        {
            _onSuccess = onSuccessFunc;
        }

        /// <summary>
        /// Optional override on failed validation.
        /// </summary>
        /// <param name="onFailureFunc"></param>
        public void OnFailure( Func<TCommand, Task<ValidationResult>> onFailureFunc )
        {
            _onFailure = onFailureFunc;
        }

        /// <summary>
        /// Handles the command - performs initial validation via the request's validator, then consumes the optional
        /// Validation hook. Returns a failure upon either validation failing, otherwise consumes the Success hook and
        /// returns success.
        /// </summary>
        /// <param name="request">Used as an input for the provided hooks.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An <see cref="ICommandResult"/> pertaining to the success or failure (with error keys) of the command.</returns>
        public async Task<ICommandResult> Handle( TCommand request, CancellationToken cancellationToken )
        {
            var requestValidationResult = request.Validate( );
            if ( !requestValidationResult.IsValid )
            {
                return Failure( requestValidationResult );
            }

            // Handler level validation
            if ( _validation != null )
            {
                var handlerValidationResult = await _validation.Invoke( request );
                if ( !handlerValidationResult.IsValid )
                {
                    if ( _onFailure != null )
                    {
                        return Failure( await _onFailure.Invoke( request ) );
                    }

                    return Failure( handlerValidationResult );
                }
            }

            Throw.If.Null( _onSuccess, nameof( _onSuccess ) );

            await _onSuccess.Invoke( request );

            // return Successful
            return Success( );
        }

        private static CommandResult Failure( ValidationResult result )
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

        private static CommandResult Success( )
        {
            return new CommandResult( );
        }
    }
}