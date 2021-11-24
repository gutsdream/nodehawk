using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.CommandHandling.Nodes;
using Application.Models.Dtos;
using Application.QueryHandling.Nodes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class NodesController : ApiBaseController
    {
        public NodesController( IMediator mediator ) : base( mediator )
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<NodeDto>>> GetNodes( )
        {
            return Evaluate( await Mediator.Send( new NodeList.Query( ) ) );
        }

        [HttpGet( "{id:guid}" )]
        public async Task<ActionResult<NodeDto>> GetNode( Guid id )
        {
            return Evaluate( await Mediator.Send( new NodeDetails.Query( id ) ) );
        }

        [HttpPost]
        public async Task<IActionResult> CreateNode( CreateNode.Command createNodeCommand )
        {
            return Evaluate( await Mediator.Send( createNodeCommand ) );
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNode( UpdateNode.Command updateNodeCommand )
        {
            return Evaluate( await Mediator.Send( updateNodeCommand ) );
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteNode( DeleteNode.Command deleteNodeCommand )
        {
            return Evaluate( await Mediator.Send( deleteNodeCommand ) );
        }
    }
}