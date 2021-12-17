using Domain.Constants;
using Domain.ExceptionHandling;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Represents an immutable null or 40 character string. 
    /// </summary>
    public class NodeExternalId
    {
        public string Value { get; }
        
        public NodeExternalId( string externalId )
        {
            if ( externalId == null )
            {
                Value = null;
            }
            else
            {
                Throw.If.InvalidLength( externalId, nameof( externalId ), NodeConstants.ExternalIdLength );
                Value = externalId;
            }
        }
    }
}