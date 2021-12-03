using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core.JobManagement;
using Domain.Interfaces;
using MediatR;

namespace Application.Core.Features.JobManagement.ActiveJobs
{
    public class GetActiveJobs
    {
        public class Query : IRequest<List<JobActivityDto>>
        {
        }
    }

    public class GetActiveJobsHandler : IRequestHandler<GetActiveJobs.Query, List<JobActivityDto>>
    {
        private readonly ActiveJobManager _activeJobManager;

        public GetActiveJobsHandler( ActiveJobManager activeJobManager )
        {
            _activeJobManager = activeJobManager;
        }

        public async Task<List<JobActivityDto>> Handle( GetActiveJobs.Query request, CancellationToken cancellationToken )
        {
            return _activeJobManager.JobActivities
                .Select( x => new JobActivityDto( x ) )
                .ToList( );
        }
    }
    
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