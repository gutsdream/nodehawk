using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Application.Core.JobState
{
    // TODO: yoink job is ex is thrown somehow
    public class JobActivityManager
    {
        private readonly List<IJobActivity> _jobActivities = new( );

        public ReadOnlyCollection<IJobActivity> JobActivities => _jobActivities.AsReadOnly( );

        public JobActivityManager( )
        {
            _jobActivities.Add( new FakeActivity( ) );
            _jobActivities.Add( new FakeActivity( ) );
        }

        public void RegisterActivity( IJobActivity activity )
        {
            _jobActivities.Add( activity );
        }

        public void CompleteActivity( IJobActivity activity )
        {
            _jobActivities.RemoveAll( x => x.Id == activity.Id );
        }

        public class FakeActivity : IJobActivity
        {
            public Guid Id { get; }
            public string Title => $"Fake Job Activity: {Id}";
            public string Status => "I'm here for frontend testing and will soon die!";

            public FakeActivity( )
            {
                Id = Guid.NewGuid( );
            }
        }
    }
}