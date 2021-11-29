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

            public static class Aws
            {
                public static Message RunBackupScript( string accessKey, string secretKey, string bucketName )
                {
                    return new Message($"docker exec otnode node scripts/backup-upload-aws.js --config=/ot-node/.origintrail_noderc " +
                                       $"--configDir=/ot-node/data --backupDirectory=/ot-node/backup " +
                                       $"--AWSAccessKeyId={accessKey} --AWSSecretAccessKey={secretKey} --AWSBucketName={bucketName}");
                }
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