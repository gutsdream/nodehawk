using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Core.JobManagement
{
    /// <summary>
    /// Stores all actively running jobs in memory. Should be injected and accessed as a singleton.
    /// </summary>
    public class InMemoryActiveJobTracker
    {
        private readonly List<IActiveJob> _activeJobs = new( );

        public ReadOnlyCollection<IActiveJob> JobActivities => _activeJobs.AsReadOnly( );

        public InMemoryActiveJobTracker( )
        {
            _activeJobs.Add( new Fake( ) );
            _activeJobs.Add( new Fake( ) );
        }

        public void BeginTrackingActiveJob( IActiveJob activity )
        {
            _activeJobs.Add( activity );
        }

        // Use SignalR to notify front end
        public void EndTrackingForActiveJob( IActiveJob activity )
        {
            _activeJobs.RemoveAll( x => x.Id == activity.Id );
        }

        public class Fake : IActiveJob
        {
            public Guid Id { get; }
            public string Title => $"Fake Job Activity: {Id}";
            public string Status => "I'm here for frontend testing and will soon die!";
            public JobType JobType => JobType.Snapshot;

            public Fake( )
            {
                Id = Guid.NewGuid( );
            }
        }
    }
}