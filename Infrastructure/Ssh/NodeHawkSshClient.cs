using System;
using System.Collections.Generic;
using System.Linq;
using Application.Core.Interfaces;
using Domain.Entities;
using Renci.SshNet;

namespace Infrastructure.Ssh
{
    public class NodeHawkNodeHawkSshClient : INodeHawkSshClient, IDisposable
    {
        private readonly ICypherService _cypherService;
        private SshClient _sshClient;

        public NodeHawkNodeHawkSshClient( ICypherService cypherService )
        {
            _cypherService = cypherService;
        }

        public void ConnectToNode( Node node )
        {
            var details = node.ConnectionDetails;
            var hostname = _cypherService.Decrypt( details.Host );
            var username = _cypherService.Decrypt( details.Username );
            var key = _cypherService.Decrypt( details.Key );

            // TODO: use a factory here rather than newing one up
            _sshClient = new SshClient( hostname, username, key );
            _sshClient.Connect( );
        }

        public ISshCommandResult Run( SshMessage nodeHawkSshMessage )
        {
            if ( _sshClient == null || !_sshClient.IsConnected )
            {
                throw new InvalidOperationException( $"Must run {nameof( ConnectToNode )} before sending SSH Messages" );
            }

            var command = _sshClient.RunCommand( nodeHawkSshMessage.Value );
            return new SshCommandResult( command );
        }

        public ISshCommandResult Run( List<SshMessage> nodeHawkSshMessages )
        {
            var commands = new List<SshCommand>( );
            foreach ( var message in nodeHawkSshMessages )
            {
                commands.Add( _sshClient.RunCommand( message.Value ) );
            }

            // We usually only really care about the final command, the others are usually just preconditions
            return new SshCommandResult( commands.Last( ) );
        }

        public void Dispose( )
        {
            _sshClient?.Disconnect( );
            _sshClient?.Dispose( );
        }
    }

    public class SshCommandResult : ISshCommandResult
    {
        public bool IsSuccessful => string.IsNullOrWhiteSpace( Error );
        public string Content { get; }
        public string Error { get; }

        public SshCommandResult( SshCommand command )
        {
            Content = command.Result;
            Error = command.Error;
        }
    }
}