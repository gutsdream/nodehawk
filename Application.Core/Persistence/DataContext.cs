using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Persistence
{
    // For now this is being used directly by all the different application services
    // They're all relatively thin and self contained vertical slices so creating a !worthwhile! abstraction over this for now is unnecessary overhead IMO
    // This dependency will be reversed at a later date if necessary, unlikely however
    public class DataContext : DbContext
    {
        public DataContext( DbContextOptions options ) : base( options )
        {
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            modelBuilder.Entity<ConnectionDetails>( )
                .HasOne( x => x.Node )
                .WithOne( x => x.ConnectionDetails )
                .HasForeignKey<Node>( x => x.ConnectionDetailsId );

            modelBuilder.Entity<Node.Snapshot>( ).ToTable( "NodeSnapshots" );
            
            base.OnModelCreating( modelBuilder );
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<Node.Snapshot> NodeSnapshots { get; set; }
        public DbSet<AwsDetails> AwsDetails { get; set; }
    }
}