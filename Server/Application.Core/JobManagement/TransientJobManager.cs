using System;
using System.Collections.Generic;
using System.Linq;
using Application.Core.Persistence;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Core.JobManagement
{
    /// <summary>
    /// Manages interactions with <see cref="InMemoryActiveJobTracker"/>, persists jobs upon completion.
    /// Should be retrieved from <see cref="TransientJobManagerFactory"/> and wrapped in a using statement to ensure disposal is carried out, unless you want shit to not work properly :D
    /// </summary>
    public class TransientJobManager : IDisposable
    {
        private readonly InMemoryActiveJobTracker _jobTracker;
        private readonly DataContext _context;
        
        private readonly List<IActiveJob> _jobsRegisteredInScope = new( );
        
        public TransientJobManager( InMemoryActiveJobTracker jobTracker, DataContext context )
        {
            _jobTracker = jobTracker;
            _context = context;
        }

        public void RegisterActiveJob( IActiveJob activity )
        {
            _jobsRegisteredInScope.Add( activity );
            _jobTracker.BeginTrackingActiveJob( activity );
        }

        /// <summary>
        /// Removes the activity from the in memory tracker, persists it as a success and sends a notification.
        /// </summary>
        public void MarkJobAsSuccess( IActiveJob activity )
        {
            var job = FinalizedJob.Success( activity );
            CompleteJob( activity, job );
        }
        
        /// <summary>
        /// Removes the activity from the in memory tracker, persists it as a failure and sends a notification.
        /// </summary>
        private void MarkJobAsFailure( IActiveJob activity )
        {
            var job = FinalizedJob.Failure( activity );
            CompleteJob( activity, job );
        }

        private void CompleteJob( IActiveJob activity, FinalizedJob job )
        {
            // Remove from this object and in the singleton tracker
            _jobsRegisteredInScope.RemoveAll( x => x.Id == activity.Id );
            _jobTracker.EndTrackingForActiveJob( activity );
            
            // Persist job
            _context.FinalizedJobs.Add( job );
            _context.SaveChanges( );
        }

        // Using Dispose as DisposeAsync is not supported by HangFire currently - may look into creating PR for this.
        public void Dispose( )
        {
            // Copying list so we can enumerate over a fresh collection whilst messing with the original. She'll be right 
            var jobsForFailure = _jobsRegisteredInScope.ToList();
            
            foreach ( var jobToFail in jobsForFailure )
            {
                MarkJobAsFailure( jobToFail );
            }
        }
    }
}