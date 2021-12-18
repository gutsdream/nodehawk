using Domain.ValueObjects.Generics;

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

        public ConnectionDetails( NonNull<string> host, NonNull<string> username, NonNull<string> key ) : this()
        {
            Host = host.Value;
            Username = username.Value;
            Key = key.Value;
        }
    }
}