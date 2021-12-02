using Application.Core.Interfaces;

namespace Application.Core.Models.Dtos
{
    public class JobActivityDto
    {
        public string Title { get; }
        public string Status { get; }

        public JobActivityDto( IJobActivity activity )
        {
            Title = activity.Title;
            Status = activity.Status;
        }
    }
}