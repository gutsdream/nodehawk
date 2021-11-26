using System;
using Domain.ExceptionHandling;

namespace Domain.Entities
{
    public class ConnectionDetails : Entity
    {
        public string Host { get; protected set; }
        public string Username { get; protected set; }
        public string Key { get; protected set; }

        public virtual Node Node { get; protected set; }

        protected ConnectionDetails( )
        {
        }

        public ConnectionDetails( string host, string username, string key ) : this()
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