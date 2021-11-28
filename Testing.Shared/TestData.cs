using System;
using Domain.Entities;

namespace Testing.Shared
{
    public static class TestData
    {
        public static class Create
        {
            public static Node Node( )
            {
                return new Node( "title", ConnectionDetails( ) ) { Id = Guid.NewGuid( ) };
            }

            public static ConnectionDetails ConnectionDetails( )
            {
                return new ConnectionDetails( "host", "username", "key" ) { Id = Guid.NewGuid( ) };
            }
        }
    }
}