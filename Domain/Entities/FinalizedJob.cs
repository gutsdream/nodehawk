using System;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class FinalizedJob : Entity
    {
        public string Title { get; protected set; }
        public bool WasSuccessful { get; protected set; }
        public JobType JobType { get; protected set; }
        
        public DateTime DateFinishedUtc { get; protected set; }

        protected FinalizedJob( )
        {
        }

        protected FinalizedJob( IActiveJob activeJob, bool isSuccess ) 
        {
            Title = activeJob.Title;
            WasSuccessful = isSuccess;
            JobType = activeJob.JobType;
            DateFinishedUtc = DateTime.UtcNow;
        }

        public static FinalizedJob Success( IActiveJob activeJob )
        {
            return new FinalizedJob( activeJob, true );
        }
        
        public static FinalizedJob Failure( IActiveJob activeJob )
        {
            return new FinalizedJob( activeJob, false );
        }
    }
}