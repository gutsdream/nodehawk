using Domain.ExceptionHandling;

namespace Application.Constants
{
    public static class SshCommands
    {
        public static readonly Command GetDiskSpace = new( "df ." );
        public static readonly Command IsContainerRunning = new( "docker container inspect -f '{{.State.Running}}' otnode" );
        
        public class Command
        {
            public string Value { get; }

            protected internal Command( string value )
            {
                Throw.If.Null( value, nameof( value ) );

                Value = value;
            }
        }
    }
}