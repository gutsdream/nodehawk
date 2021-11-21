using System;

namespace Domain.Entities
{
    public class Node
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        
        /// <summary>
        /// The Node's identifier (allows for connection to OT Hub)
        /// </summary>
        public string ExternalId { get; set; }
    }
}