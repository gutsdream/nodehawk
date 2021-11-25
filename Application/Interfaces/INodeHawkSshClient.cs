using Application.Constants;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface INodeHawkSshClient
    {
        void ConnectToNode( Node node );
        ISshCommandResult Run( SshCommands.Command command );
    }

    public interface ISshCommandResult
    {
        string Content { get; }
        string Error { get; }
        
        bool IsSuccessful { get; }
    }
}