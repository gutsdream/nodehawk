using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Application.Core.Features.Nodes.Commands.Create;
using Application.Core.Persistence;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Core.JobManagement
{
    /// <summary>
    /// Manages interactions with <see cref="InMemoryActiveJobTracker"/>, persists jobs upon completion. Should be injected on a per scope basis unless you want all of this to go horribly wrong :D
    /// </summary>
    public class TransientJobManagerFactory {
        private readonly InMemoryActiveJobTracker _jobTracker;
        private readonly DataContext _context;

        public TransientJobManagerFactory( InMemoryActiveJobTracker jobTracker, DataContext context )
        {
            _jobTracker = jobTracker;
            _context = context;
        }

        public TransientJobManager Create( )
        {
            return new TransientJobManager( _jobTracker, _context );
        }
    }
}