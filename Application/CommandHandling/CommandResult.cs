using System.Collections.Generic;
using System.Linq;

namespace Application.CommandHandling
{
    public class CommandResult : ICommandResult
    {
        public bool IsSuccessful => Errors.Any( );
        public List<Error> Errors { get; }

        public CommandResult( )
        {
            Errors = new List<Error>( );
        }

        public void AddError( Error error )
        {
            Errors.Add(error);
        }
    }
}