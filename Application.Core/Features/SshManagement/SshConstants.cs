namespace Application.Core.Features.SshManagement
{
    public static class SshConstants
    {
        public const string GetSpaceTakenOnDrive = "df .";
        public const string CheckIfContainerIsRunning = "docker container inspect -f '{{.State.Running}}' otnode";
        
        
    }
}