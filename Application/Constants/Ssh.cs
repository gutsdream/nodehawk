using System.Collections.Generic;
using Domain.ExceptionHandling;

namespace Application.Constants
{
    public static class Ssh
    {
        public class Queries
        {
            public static readonly Message GetDiskSpace = new("df .");
            public static readonly Message IsContainerRunning = new("docker container inspect -f '{{.State.Running}}' otnode");
        }

        public class Commands
        {
            public static class SpaceManagement
            {
                public static readonly Message DeleteDockerOtNodeLogFile = new("truncate -s 0 $(docker inspect -f '{{.LogPath}}' otnode)");

                public static readonly List<Message> DeleteAllDockerTextLogs = new( )
                {
                    new("cd  /var/lib/docker/overlay2"),
                    new("find . -type f -name \"*.log\" -delete")
                };

                public static readonly List<Message> CleanCacheAndJournals = new( )
                {
                    new("apt clean"),
                    new("journalctl --vacuum-time=1h")
                };
            }
        }

        public class Message
        {
            public string Value { get; }

            protected internal Message( string value )
            {
                Throw.If.Null( value, nameof( value ) );

                Value = value;
            }
        }
    }
}