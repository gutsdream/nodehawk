using System;
using System.Collections.Generic;
using System.Linq;
using Application.Interfaces;
using MediatR;
using Moq;

namespace Application.Testing.Mocks
{
    public class BackgroundTaskManagerMock : Mock<IBackgroundTaskManager>
    {
        private readonly List<object> _queuedRequests = new( );

        public BackgroundTaskManagerMock( )
        {  
        }

        // TODO: can we do this in a not aids way :D
        public void ConfigureQueue<TRequest, TResult>( ) where TRequest : IRequest<TResult>
        {
            Setup( x => x.QueueRequest<TRequest, TResult>( It.IsAny<TRequest>( ) ) ).Callback( ( TRequest request ) =>
            {
                _queuedRequests.Add( request );
            } );
        }

        public bool ContainsRequestType<TRequestType>( )
        {
            return _queuedRequests.OfType<TRequestType>( ).Any( );
        }
    }

    [TypeMatcher]
    public class IsQueueableRequest<TRequest, TResult> : ITypeMatcher
        where TRequest : IRequest<TResult>
    {
        public bool Matches( Type typeArgument )
        {
            return true;
        }
    };
}