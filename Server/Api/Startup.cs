using Api.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Scheduler;
using Scheduler.SnapshotScheduler;

namespace Api
{
    public class Startup
    {
        private const string CorsPolicyName = "CorsPolicy";

        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddControllers( )
                .AddNewtonsoftJson( x => { x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; } );

            services.AddSwaggerGen( c =>
            {
                c.SwaggerDoc( "v1", new OpenApiInfo { Title = "Api", Version = "v1" } );
                c.CustomSchemaIds( type => type.ToString( ) );
            } );
            
            services.AddCors( x => x.AddPolicy( CorsPolicyName, policy =>
            {
                policy.AllowAnyMethod( )
                    .AllowAnyHeader( )
                    .WithOrigins( "http://localhost:3000" );
            } ) );

            services.AddApplicationServices( Configuration );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app,
            IWebHostEnvironment env,
            IRecurringJobManager recurringJobManager,
            IBackgroundJobClient backgroundJobManager,
            NodeHawkScheduledJobs nodeHawkScheduledJobs )
        {
            if ( env.IsDevelopment( ) )
            {
                app.UseDeveloperExceptionPage( );
                app.UseSwagger( );
                app.UseSwaggerUI( c => c.SwaggerEndpoint( "/swagger/v1/swagger.json", "Api v1" ) );
            }

            app.UseRouting( );

            app.UseCors( CorsPolicyName );

            app.UseAuthorization( );

            app.UseEndpoints( endpoints => { endpoints.MapControllers( ); } );

            JobRegistration.Initialize( recurringJobManager, backgroundJobManager, nodeHawkScheduledJobs );
        }
    }
}