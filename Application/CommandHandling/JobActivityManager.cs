using System.Collections.Generic;
using System.Collections.ObjectModel;
using Application.Interfaces;

namespace Application.CommandHandling
{
    public class JobActivityManager
    {
        private readonly List<IJobActivity> _jobActivities = new( );

        public ReadOnlyCollection<IJobActivity> JobActivities => _jobActivities.AsReadOnly( );

        public void RegisterActivity( IJobActivity activity )
        {
            _jobActivities.Add( activity );
        }

        public void CompleteActivity( IJobActivity activity )
        {
            _jobActivities.RemoveAll( x => x.Id == activity.Id );
        }
    }
}