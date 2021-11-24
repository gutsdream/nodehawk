using System;
using Domain.ExceptionHandling;

namespace Domain.Entities
{
    public class ConnectionDetails
    {
        public Guid Id { get; protected set; }
        public Guid NodeId { get; protected set; }
        public string Host { get; protected set; }
        public string Username { get; protected set; }
        public string Key { get; protected set; }

        public virtual Node Node { get; protected set; }

        protected ConnectionDetails( )
        {
        }

        // TODO: encryption
        public ConnectionDetails( string host, string username, string key )
        {
            Throw.If.Null( host, nameof( host ) );
            Throw.If.Null( username, nameof( username ) );
            Throw.If.Null( key, nameof( key ) );

            Host = host;
            Username = username;
            Key = key;
        }
    }
}