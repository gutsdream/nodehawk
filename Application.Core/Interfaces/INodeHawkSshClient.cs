using System.Collections.Generic;
using Domain.Entities;
using Domain.ExceptionHandling;

namespace Application.Core.Interfaces
{
    public interface INodeHawkSshClient
    {
        void ConnectToNode( Node node );
        ISshCommandResult Run( SshMessage nodeHawkSshSshMessage );
        ISshCommandResult Run( List<SshMessage> nodeHawkSshMessage );
    }
    
    public class SshMessage
    {
        public string Value { get; }

        public SshMessage( string value )
        {
            Throw.If.Null( value, nameof( value ) );

            Value = value;
        }
    }

    public interface ISshCommandResult
    {
        string Content { get; }
        string Error { get; }
        
        bool IsSuccessful { get; }
    }
}