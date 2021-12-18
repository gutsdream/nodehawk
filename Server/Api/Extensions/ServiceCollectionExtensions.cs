using Application.Core.Features.Nodes.Queries.NodeList;
using Application.Core.Interfaces;
using Application.Core.JobManagement;
using Hangfire;
using Hangfire.MemoryStorage;
using Infrastructure.Encryption;
using Infrastructure.Ssh;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Core.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Scheduler;
using Scheduler.SnapshotScheduler;

namespace Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices( this IServiceCollection services, IConfiguration configuration )
        {
            services.AddDbContext<DataContext>( opt => { opt.UseSqlite( configuration.GetConnectionString( "DefaultConnection" ) ); } );

            services.AddDataProtection( )
                .SetApplicationName( "nodehawk" );

            services.AddSingleton<ICypherService, CypherService>( );
            services.AddScoped<INodeHawkSshClient, NodeHawkSshClient>( );

            services.AddMediatR( typeof( NodeListQueryHandler ).Assembly );

            services.AddHangfire( x => { 
                x.UseMemoryStorage( );
            } );
            services.AddHangfireServer( );
            
            services.AddScoped<NodeHawkScheduledJobs, NodeHawkScheduledJobs>( );
            services.AddScoped<IEventManager, EventManager>( );

            services.AddSingleton<InMemoryActiveJobTracker, InMemoryActiveJobTracker>( );
            services.AddScoped<TransientJobManagerFactory, TransientJobManagerFactory>( );
        }
    }
}