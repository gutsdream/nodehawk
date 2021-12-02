using Application.Core.Models.Results;
using Xunit;

namespace Application.Testing
{
    public static class Then
    {
        public static void ResultContainsError( ICommandResult result, string key, string message )
        {
            Assert.Contains( result.Errors, x => x.Key == key && x.Message == message );
        }
    }
}