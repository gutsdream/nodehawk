using Application.CommandHandling;
using Application.Interfaces;
using Application.QueryHandling.Nodes;
using Hangfire;
using Hangfire.MemoryStorage;
using Infrastructure.Encryption;
using Infrastructure.Ssh;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Scheduler;
using Scheduler.SnapshotScheduler;

namespace Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices( this IServiceCollection services, IConfiguration configuration )
        {
            services.AddDbContext<DataContext>( opt => { opt.UseSqlite( configuration.GetConnectionString( "DefaultConnection" ) ); } );

            services.AddDataProtection( );

            services.AddScoped<IRepository, Repository>( );
            services.AddSingleton<ICypherService, CypherService>( );
            services.AddScoped<INodeHawkSshClient, NodeHawkNodeHawkSshClient>( );

            services.AddMediatR( typeof( NodeListQueryHandler ).Assembly );

            services.AddHangfire( x => { x.UseMemoryStorage( ); } );
            services.AddHangfireServer( );
            
            services.AddScoped<NodeHawkScheduledJobs, NodeHawkScheduledJobs>( );
            services.AddScoped<IBackgroundTaskManager, BackgroundTaskManager>( );

            services.AddSingleton<JobActivityManager, JobActivityManager>( );
        }
    }
}