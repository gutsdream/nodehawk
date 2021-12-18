using Application.Core.Persistence;

namespace Application.Core.JobManagement
{
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