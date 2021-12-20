using System.Threading.Tasks;
using Application.Core.Features.Aws.AwsDetails.Register;
using Application.Core.Features.Aws.AwsDetails.Update;
using Application.Core.Features.Aws.BackupNode;
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
        
        [HttpPost]
        public async Task<IActionResult> BulkBackupNode( BulkBackupNode.Command updateAwsDetails )
        {
            await Mediator.Send( updateAwsDetails );

            // Returning OK 200 response as it's not worth concatenating errors for a bulk command IMO, job history displays it
            return Ok( );
        }
    }
}