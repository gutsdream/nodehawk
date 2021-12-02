using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.ExceptionHandling;

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

        public DateTime? LastBackupDateUtc { get; protected set; }
        public DateTime? LastCleanedDateUtc { get; protected set; }
        public DateTime? LastSnapshotDateUtc { get; protected set; }

        public virtual ConnectionDetails ConnectionDetails { get; protected set; }
        public virtual ICollection<Snapshot> Snapshots { get; protected set; }

        protected Node( )
        {
            Snapshots = new HashSet<Snapshot>( );
        }

        public Node( string title, ConnectionDetails connectionDetails, string externalId = null ) : this()
        {
            SetTitle( title );
            SetConnectionDetails( connectionDetails );
            SetExternalId( externalId );
        }

        public void SetTitle( string title )
        {
            Throw.If.Null( title, nameof( title ) );
            Title = title;
        }

        public void SetConnectionDetails( ConnectionDetails connectionDetails )
        {
            Throw.If.Null( connectionDetails, nameof( connectionDetails ) );
            ConnectionDetails = connectionDetails;
        }

        public void SetExternalId( string externalId )
        {
            if ( externalId == null )
            {
                return;
            }

            Throw.If.InvalidLength( externalId, nameof( externalId ), NodeConstants.ExternalIdLength );
            ExternalId = externalId;
        }

        public void CreateSnapshot( int spaceUsedPercentage, bool containerRunning )
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

            protected internal Snapshot( Node node, int spaceUsedPercentage, bool containerRunning ) : this()
            {
                Node = node;
                ContainerRunning = containerRunning;

                SetSpaceUsed( spaceUsedPercentage );
            }

            private void SetSpaceUsed( int spaceUsed )
            {
                Throw.If.Null( spaceUsed, nameof( spaceUsed ) );
                Throw.IfNot.Percentage( spaceUsed, nameof( spaceUsed ) );
                SpaceUsedPercentage = spaceUsed;
            }
        }
    }
}