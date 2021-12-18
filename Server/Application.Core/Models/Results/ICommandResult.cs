using System.Collections.Generic;

namespace Application.Core.Models.Results
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
}