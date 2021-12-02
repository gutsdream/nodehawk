using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Core.Features.Nodes.Commands.Create;
using Application.Core.Features.Nodes.Commands.Delete;
using Application.Core.Features.Nodes.Commands.Update;
using Application.Core.Features.Nodes.Queries.NodeDetails;
using Application.Core.Features.Nodes.Queries.NodeList;
using Application.Core.Features.SshManagement.Snapshots.Create;
using Application.Core.Features.SshManagement.SpaceManagement.Clean;
using Application.Core.Models.Dtos;
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
        public async Task<IActionResult> Create( CreateNode.Command createNodeCommand )
        {
            return Evaluate( await Mediator.Send( createNodeCommand ) );
        }

        [HttpPost]
        public async Task<IActionResult> Update( UpdateNode.Command updateNodeCommand )
        {
            return Evaluate( await Mediator.Send( updateNodeCommand ) );
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete( DeleteNode.Command deleteNodeCommand )
        {
            return Evaluate( await Mediator.Send( deleteNodeCommand ) );
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSnapshot( CreateNodeSnapshot.Command createNodeSnapshotCommand )
        {
            return Evaluate( await Mediator.Send( createNodeSnapshotCommand ) );
        }
        
        [HttpPost]
        public async Task<IActionResult> CleanNode( CleanNode.Command cleanNodeCommand )
        {
            return Evaluate( await Mediator.Send( cleanNodeCommand ) );
        }
    }
}