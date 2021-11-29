using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.CommandHandling;
using Application.Models.Dtos;
using MediatR;

namespace Application.QueryHandling.JobActivities
{
    public class GetActiveJobs
    {
        public class Query : IRequest<List<JobActivityDto>>
        {
        }
    }

    public class GetActiveJobsHandler : IRequestHandler<GetActiveJobs.Query, List<JobActivityDto>>
    {
        private readonly JobActivityManager _jobActivityManager;

        public GetActiveJobsHandler( JobActivityManager jobActivityManager )
        {
            _jobActivityManager = jobActivityManager;
        }

        public async Task<List<JobActivityDto>> Handle( GetActiveJobs.Query request, CancellationToken cancellationToken )
        {
            return _jobActivityManager.JobActivities
                .Select( x => new JobActivityDto( x ) )
                .ToList( );
        }
    }
}