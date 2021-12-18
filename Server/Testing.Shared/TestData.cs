using System;
using Application.Core.JobManagement;
using Application.Core.Persistence;
using Domain.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Generics;
using Microsoft.EntityFrameworkCore;

namespace Testing.Shared
{
    public static class TestData
    {
        public static class Create
        {
            public static Node Node( )
            {
                return new Node( "title".AsNonNull( ), ConnectionDetails( ).AsNonNull( ), new NodeExternalId( null ) ) { Id = Guid.NewGuid( ) };
            }

            public static ConnectionDetails ConnectionDetails( )
            {
                return new ConnectionDetails( "host".AsNonNull( ), "username".AsNonNull( ), "key".AsNonNull( ) ) { Id = Guid.NewGuid( ) };
            }

            public static DataContext UniqueContext( )
            {
                var dbContextOptions = new DbContextOptionsBuilder<DataContext>( )
                    .UseInMemoryDatabase( databaseName: Guid.NewGuid( ).ToString( ) )
                    .Options;

                return new DataContext( dbContextOptions );
            }

            public static TransientJobManagerFactory JobManagerFactory( DataContext context )
            {
                return new TransientJobManagerFactory( new InMemoryActiveJobTracker( ), context );
            }
        }
    }
}