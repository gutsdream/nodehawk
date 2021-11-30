using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

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

            //TODO: move
            public static DataContext UniqueContext( )
            {
                var dbContextOptions = new DbContextOptionsBuilder<DataContext>( )
                    .UseInMemoryDatabase( databaseName: Guid.NewGuid( ).ToString( ) )
                    .Options;

                return new DataContext( dbContextOptions );
            }
        }
    }
}