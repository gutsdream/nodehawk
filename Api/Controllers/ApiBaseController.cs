using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiBaseController : ControllerBase
    {
        public IMediator Mediator { get; }

        public ApiBaseController( IMediator mediator )
        {
            Mediator = mediator;
        }
    }
}