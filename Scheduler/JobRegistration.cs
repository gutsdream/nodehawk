using System;
using System.Threading.Tasks;
using Application.CommandHandling.Nodes;
using Application.CommandHandling.Nodes.Snapshots;
using Hangfire;
using MediatR;
using Scheduler.SnapshotScheduler;

namespace Scheduler
{
    public static class JobRegistration
    {
        public static void Initialize( IRecurringJobManager recurringJobManager,
            IBackgroundJobClient backgroundJobManager,
            NodeHawkScheduledJobs nodeHawkScheduledJobs )
        {
            backgroundJobManager.Enqueue( ( ) => nodeHawkScheduledJobs.SnapshotAll( ) );
            recurringJobManager.AddOrUpdate( "Snapshot Generation: 15 Minute Interval",
                ( ) => nodeHawkScheduledJobs.SnapshotAll( ),
                "*/15 * * * *" );

            backgroundJobManager.Enqueue( ( ) => nodeHawkScheduledJobs.CleanOldSnapshots( ) );
            recurringJobManager.AddOrUpdate( "Node Snapshot Cleanup: Daily",
                ( ) => nodeHawkScheduledJobs.CleanOldSnapshots( ),
                Cron.Daily );
        }
    }
}