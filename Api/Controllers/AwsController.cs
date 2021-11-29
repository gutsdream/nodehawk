using System.Threading.Tasks;
using Application.CommandHandling.Aws;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class AwsController : ApiBaseController
    {
        public AwsController( IMediator mediator ) : base( mediator )
        {
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDetails( RegisterAwsDetails.Command registerAwsDetails )
        {
            return Evaluate( await Mediator.Send( registerAwsDetails ) );
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateDetails( UpdateAwsDetails.Command updateAwsDetails )
        {
            return Evaluate( await Mediator.Send( updateAwsDetails ) );
        }
        
        [HttpPost]
        public async Task<IActionResult> BackupNode( BackupNode.Command updateAwsDetails )
        {
            return Evaluate( await Mediator.Send( updateAwsDetails ) );
        }
    }
}