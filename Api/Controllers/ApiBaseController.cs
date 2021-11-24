using Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public abstract class ApiBaseController : ControllerBase
    {
        public IMediator Mediator { get; }

        public ApiBaseController( IMediator mediator )
        {
            Mediator = mediator;
        }

        [ApiExplorerSettings( IgnoreApi = true )]
        public ActionResult Evaluate( ICommandResult result )
        {
            return result.IsSuccessful
                ? Ok( result )
                : BadRequest( result );
        }

        [ApiExplorerSettings( IgnoreApi = true )]
        public ActionResult<TContent> Evaluate<TContent>( IQueryResult<TContent> result )
        {
            return result.State == Status.Found
                ? Ok( result.Content )
                : NotFound( result.Content );
        }
    }
}