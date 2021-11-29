using System.Collections.Generic;
using Application.Constants;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface INodeHawkSshClient
    {
        void ConnectToNode( Node node );
        ISshCommandResult Run( Ssh.Message nodeHawkSshMessage );
        ISshCommandResult Run( List<Ssh.Message> nodeHawkSshMessage );
    }

    public interface ISshCommandResult
    {
        string Content { get; }
        string Error { get; }
        
        bool IsSuccessful { get; }
    }
}