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

        public ConnectionDetails( )
        {
        }

        public ConnectionDetails( string host, string username, string key )
        {
            Throw.IfNull( host, nameof( host ) );
            Throw.IfNull( username, nameof( username ) );
            Throw.IfNull( key, nameof( key ) );

            Host = host;
            Username = username;
            Key = key;
        }
    }
}