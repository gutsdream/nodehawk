using System;
using System.Collections.Generic;
using Domain.Constants;
using Domain.ExceptionHandling;

namespace Domain.Entities
{
    public class Node
    {
        public Guid Id { get; protected set; }
        public Guid ConnectionDetailsId { get; protected set; }

        public string Title { get; set; }

        /// <summary>
        /// The Node's identifier (allows for connection to OT Hub) - optional
        /// </summary>
        public string ExternalId { get; set; }

        public virtual ConnectionDetails ConnectionDetails { get; protected set; }
        public virtual ICollection<Snapshot> Snapshots { get; protected set; }

        protected Node( )
        {
            Snapshots = new HashSet<Snapshot>( );
        }

        public Node( string title, ConnectionDetails connectionDetails, string externalId = null )
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

        public Snapshot CreateSnapshot( int spaceUsedPercentage )
        {
            var snapshot = new Snapshot( this, spaceUsedPercentage );
            Snapshots.Add( snapshot );

            return snapshot;
        }

        public class Snapshot
        {
            public Guid Id { get; protected set; }
            public Guid NodeId { get; protected set; }

            public int SpaceUsedPercentage { get; protected set; }
            public int SpaceAvailablePercentage => 100 - SpaceUsedPercentage;

            public DateTime CreatedDateUtc { get; protected set; }

            public virtual Node Node { get; protected set; }

            protected Snapshot( )
            {
                CreatedDateUtc = DateTime.UtcNow;
            }

            protected internal Snapshot( Node node, int spaceUsedPercentage )
            {
                Throw.If.Null( node, nameof( node ) );
                Node = node;

                SetSpaceUsed( spaceUsedPercentage );
            }

            public void SetSpaceUsed( int spaceUsed )
            {
                Throw.If.Null( spaceUsed, nameof( spaceUsed ) );
                Throw.IfNot.Percentage( spaceUsed, nameof( spaceUsed ) );
                SpaceUsedPercentage = spaceUsed;
            }
        }
    }
}