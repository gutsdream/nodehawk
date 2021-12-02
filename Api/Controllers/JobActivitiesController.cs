using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Core.Features.JobManagement.ActiveJobs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class JobActivitiesController : ApiBaseController
    {
        public JobActivitiesController( IMediator mediator ) : base( mediator )
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<JobActivityDto>>> GetActiveJobs( )
        {
            return await Mediator.Send( new GetActiveJobs.Query( ) );
        }
    }
}