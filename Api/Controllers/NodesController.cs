using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Api.Controllers
{
    public class NodesController : ApiBaseController
    {
        private readonly DataContext _context;

        public NodesController( DataContext context )
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Node>>> GetNodes( )
        {
            return await _context.Nodes.ToListAsync( );
        }

        [HttpGet("{id}")]

    public async Task<ActionResult<Node>> GetNode( Guid id )
        {
            return await _context.Nodes.FindAsync( id );
        }
    }
}