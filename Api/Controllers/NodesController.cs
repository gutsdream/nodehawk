using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Nodes;
using Application.Nodes.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class NodesController : ApiBaseController
    {
        public NodesController( IMediator mediator ) : base( mediator ) { }
        
        [HttpGet]
        public async Task<ActionResult<List<Node>>> GetNodes( )
        {
            return await Mediator.Send( new NodeList.Query( ) );
        }

        [HttpGet( "{id:guid}" )]
        public async Task<ActionResult<Node>> GetNode( Guid id )
        {
            return await Mediator.Send( new NodeDetails.Query( id ) );
        }
    }
}