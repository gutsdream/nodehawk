using Application.Nodes.Commands.Interfaces;
using FluentValidation;

namespace Application.Validators.Nodes
{
    public class MutateNodeValidator<TAction> : AbstractValidator<TAction> where TAction : IMutateNode
    {
        public MutateNodeValidator( )
        {
            RuleFor( x => x.Title ).NotEmpty( );

            // Should either be null or properly formed
            RuleFor( x => x.ExternalId ).Length( 40 ).When( x => x.ExternalId != null );

            // Ensure SSH connection details provided
            RuleFor( x => x.Host ).NotEmpty( );
            RuleFor( x => x.Username ).NotEmpty( );
            RuleFor( x => x.Key ).NotEmpty( );
        }
    }
}