using System;
using System.Collections.Generic;
using System.Linq;
using Application.Core.Interfaces;
using Application.Core.Shared.Interfaces;
using MediatR;
using Moq;

namespace Application.Testing.Mocks
{
    public class EventManagerMock : Mock<IEventManager>
    {
        private readonly List<IApplicationEvent> _queuedEvents = new( );

        public EventManagerMock( )
        {
            Setup( x => x.PublishEvent( It.IsAny<IApplicationEvent>( ) ) ).Callback( ( IApplicationEvent applicationEvent ) =>
            {
                _queuedEvents.Add( applicationEvent );
            } );
        }

        public bool ContainsRequestType<TRequestType>( ) where TRequestType : IApplicationEvent
        {
            return _queuedEvents.OfType<TRequestType>( ).Any( );
        }
    }
}