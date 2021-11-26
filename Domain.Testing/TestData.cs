using Domain.Entities;

namespace Domain.Testing
{
    public static class TestData
    {
        public static class Create
        {
            public static Node Node( )
            {
                return new Node( "title", ConnectionDetails( ) );
            }

            public static ConnectionDetails ConnectionDetails( )
            {
                return new ConnectionDetails( "host", "username", "key" );
            }
        }
    }
}