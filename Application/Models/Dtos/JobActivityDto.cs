using Application.Interfaces;

namespace Application.Models.Dtos
{
    public class JobActivityDto
    {
        public string Title { get; set; }
        public string Status { get; set; }

        public JobActivityDto( IJobActivity activity )
        {
            Title = activity.Title;
            Status = activity.Status;
        }
    }
}