using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Application.Core.JobManagement
{
    public class ActiveJobManager
    {
        private readonly List<IActiveJob> _activeJobs = new( );

        public ReadOnlyCollection<IActiveJob> JobActivities => _activeJobs.AsReadOnly( );

        public ActiveJobManager( )
        {
            _activeJobs.Add( new Fake( ) );
            _activeJobs.Add( new Fake( ) );
        }

        public void RegisterActivity( IActiveJob activity )
        {
            _activeJobs.Add( activity );
        }

        // TODO: persist activity as successful or not, use signalr to push notifications
        public void CompleteActivity( IActiveJob activity )
        {
            _activeJobs.RemoveAll( x => x.Id == activity.Id );
        }
        
        public void FailActivity( IActiveJob activity )
        {
            _activeJobs.RemoveAll( x => x.Id == activity.Id );
        }

        public class Fake : IActiveJob
        {
            public Guid Id { get; }
            public string Title => $"Fake Job Activity: {Id}";
            public string Status => "I'm here for frontend testing and will soon die!";

            public Fake( )
            {
                Id = Guid.NewGuid( );
            }
        }
    }
}