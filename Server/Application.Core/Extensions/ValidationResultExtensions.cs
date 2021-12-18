using FluentValidation.Results;

namespace Application.Core.Extensions
{
    public static class ValidationResultExtensions
    {
        public static void AddError( this ValidationResult validationResult, string key, string message )
        {
            validationResult.Errors.Add( new ValidationFailure( key, message ) );
        }
    }
}