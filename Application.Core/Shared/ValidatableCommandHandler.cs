using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core.Interfaces;
using Application.Core.JobManagement;
using Application.Core.Models.Requests;
using Application.Core.Models.Results;
using Application.Core.Persistence;
using Application.Core.Shared.Interfaces;
using Domain.Entities;
using Domain.ExceptionHandling;
using Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Core.Shared
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
        private ActiveJobManager _activeJobManager;
        private IEventManager _eventManager;
        
        private Func<TCommand, Task<ValidationResult>> _validation;
        private Func<TCommand, Task> _onSuccess;
        
        private readonly List<IActiveJob> _activities = new( );
        private DataContext _context;

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
        /// Allows for the job subscription + publishing
        /// </summary>
        protected void UsingJobs( ActiveJobManager activeJobManager, DataContext context )
        {
            // TODO: replace with an eventing type thing when hangfire isnt a piece of dogshit
            _activeJobManager = activeJobManager;
            _context = context;
        }

        /// <summary>
        /// Registers an activity which can be updated from the implemented handler. Will fail upon an exception being thrown or otherwise succeed.
        /// </summary>
        /// <param name="activeJob"></param>
        protected void RegisterActiveJob( IActiveJob activeJob )
        {
            Throw.If.Null( _activeJobManager, nameof( ActiveJobManager ) );

            _activities.Add( activeJob );
            _activeJobManager.RegisterActivity( activeJob );
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
                    return Failure( handlerValidationResult );
                }
            }

            Throw.If.Null( _onSuccess, nameof( _onSuccess ) );

            try
            {
                await _onSuccess.Invoke( request );
            }
            catch ( Exception exception )
            {
                // We need to fail any registered job activities upon an exception being thrown
                _activities.ForEach( async x =>
                {
                    _activeJobManager.RemoveActivity( x );

                    var job = FinalizedJob.Failure( x );
                    _context.FinalizedJobs.Add( job );
                    await _context.SaveChangesAsync( );
                } );
                throw;
            }

            // Complete all registered job activities
            // TODO: replace with background event, find a way around hangfire's stupid serialization dogshit.
            _activities.ForEach( async x =>
            {
                _activeJobManager.RemoveActivity( x );
                var job = FinalizedJob.Success( x );
                _context.FinalizedJobs.Add( job );
                await _context.SaveChangesAsync( );
            } );

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