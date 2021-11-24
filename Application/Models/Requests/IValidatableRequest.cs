using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Models.Requests
{
    public abstract class ValidatableRequest<TRequest, TValidator> : IRequest<ICommandResult>
        where TValidator : AbstractValidator<TRequest>, new( )
        where TRequest : ValidatableRequest<TRequest, TValidator>
    {
        public ValidationResult Validate( )
        {
            return new TValidator( ).Validate( ( TRequest ) this );
        }
    }
}