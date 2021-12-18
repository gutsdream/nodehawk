using System;
using System.Collections.Generic;
using System.Linq;
using Domain.ValueObjects;
using Domain.ValueObjects.Generics;

namespace Domain.Entities
{
    public class Node : Entity
    {
        public Guid ConnectionDetailsId { get; protected set; }

        public string Title { get; set; }

        /// <summary>
        /// The Node's identifier (allows for connection to OT Hub) - optional
        /// </summary>
        public string ExternalId { get; set; }

        public Snapshot MostRecentSnapshot => Snapshots.OrderByDescending( x => x.CreatedDateUtc ).FirstOrDefault( );

        public DateTime CreatedDateUtc { get; protected set; }
        public DateTime? LastBackupDateUtc { get; protected set; }
        public DateTime? LastCleanedDateUtc { get; protected set; }
        public DateTime? LastSnapshotDateUtc { get; protected set; }

        public virtual ConnectionDetails ConnectionDetails { get; protected set; }
        public virtual ICollection<Snapshot> Snapshots { get; protected set; }

        protected Node( )
        {
            CreatedDateUtc = DateTime.UtcNow;
            Snapshots = new HashSet<Snapshot>( );
        }

        public Node( NonNull<string> title, NonNull<ConnectionDetails> connectionDetails, NodeExternalId externalId ) : this()
        {
            SetTitle( title );
            SetConnectionDetails( connectionDetails );
            SetExternalId( externalId );
        }

        public void SetTitle( NonNull<string> title )
        {
            Title = title.Value;
        }

        public void SetConnectionDetails( NonNull<ConnectionDetails> connectionDetails )
        {
            ConnectionDetails = connectionDetails.Value;
        }

        public void SetExternalId( NodeExternalId externalId )
        {
            ExternalId = externalId.Value;
        }

        public void CreateSnapshot( Percentage spaceUsedPercentage, bool containerRunning )
        {
            var snapshot = new Snapshot( this, spaceUsedPercentage, containerRunning );
            Snapshots.Add( snapshot );

            LastSnapshotDateUtc = DateTime.UtcNow;
        }

        public void AuditBackup( )
        {
            LastBackupDateUtc = DateTime.UtcNow;
        }
        
        public void AuditCleanup( )
        {
            LastCleanedDateUtc = DateTime.UtcNow;
        }

        public class Snapshot : Entity
        {
            public Guid NodeId { get; protected set; }

            public int SpaceUsedPercentage { get; protected set; }
            public int SpaceAvailablePercentage => 100 - SpaceUsedPercentage;
            
            public bool ContainerRunning { get; protected set; }

            public DateTime CreatedDateUtc { get; protected set; }

            public virtual Node Node { get; protected set; }

            protected Snapshot( )
            {
                CreatedDateUtc = DateTime.UtcNow;
            }

            protected internal Snapshot( Node node, Percentage spaceUsedPercentage, bool containerRunning ) : this()
            {
                Node = node;
                ContainerRunning = containerRunning;
                SetSpaceUsed( spaceUsedPercentage );
            }

            private void SetSpaceUsed( Percentage spaceUsedPercentage )
            {
                SpaceUsedPercentage = spaceUsedPercentage.Value;
            }
        }
    }
}