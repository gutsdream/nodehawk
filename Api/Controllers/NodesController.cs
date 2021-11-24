using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models.Dtos;
using Application.Nodes.Commands;
using Application.Nodes.Queries;
using Domain.Entities;
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
            return Evaluate( await Mediator.Send( new NodeDetails.Query( id ) ));
        }

        [HttpPost]
        public async Task<IActionResult> CreateNode( CreateNode.Request createNodeRequest )
        {
            return Evaluate( await Mediator.Send( createNodeRequest ) );
        }
    }
}