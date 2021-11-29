using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
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
        public DbSet<AwsDetails> AwsDetails { get; set; }
    }
}