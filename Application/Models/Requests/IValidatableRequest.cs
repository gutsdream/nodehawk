using Application.CommandHandling;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Models.Requests
{
    /// <summary>
    /// Represents a command with validatable properties
    /// </summary>
    /// <typeparam name="TCommand">The command - self reference usually</typeparam>
    /// <typeparam name="TValidator">Fluent validator corresponding to TCommand</typeparam>
    public abstract class ValidatableCommand<TCommand, TValidator> : IRequest<ICommandResult>
        where TValidator : AbstractValidator<TCommand>, new( )
        where TCommand : ValidatableCommand<TCommand, TValidator>
    {
        public ValidationResult Validate( )
        {
            return new TValidator( ).Validate( ( TCommand ) this );
        }
    }
}