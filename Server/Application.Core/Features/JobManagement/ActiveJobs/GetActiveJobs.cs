using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core.JobManagement;
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
        private readonly InMemoryActiveJobTracker _jobTracker;

        public GetActiveJobsHandler( InMemoryActiveJobTracker jobTracker )
        {
            _jobTracker = jobTracker;
        }

        public async Task<List<JobActivityDto>> Handle( GetActiveJobs.Query request, CancellationToken cancellationToken )
        {
            return _jobTracker.JobActivities
                .Select( x => new JobActivityDto( x ) )
                .ToList( );
        }
    }
}