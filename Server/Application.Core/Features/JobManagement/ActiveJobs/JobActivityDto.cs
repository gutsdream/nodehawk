using Domain.Interfaces;

namespace Application.Core.Features.JobManagement.ActiveJobs
{
    public class JobActivityDto
    {
        public string Title { get; }
        public string Status { get; }

        public JobActivityDto( IActiveJob activity )
        {
            Title = activity.Title;
            Status = activity.Status;
        }
    }
}