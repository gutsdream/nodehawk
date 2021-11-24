using System.Collections.Generic;
using System.Linq;

namespace Application
{
    public interface ICommandResult
    {
        public bool IsSuccessful { get; }
        public List<Error> Errors { get; }
    }

    public class Error
    {
        public string Key { get; }
        public string Message { get; }

        public Error( string key, string message )
        {
            Key = key;
            Message = message;
        }
    }

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