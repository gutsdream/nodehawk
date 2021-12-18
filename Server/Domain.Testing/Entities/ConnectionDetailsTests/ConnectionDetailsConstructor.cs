using Domain.Entities;
using Domain.ValueObjects.Generics;
using Xunit;

namespace Domain.Testing.Entities.ConnectionDetailsTests
{
    public class ConnectionDetailsConstructor
    {
        [Fact]
        public void Should_SuccessfullyConstructConnectionDetails( )
        {
            // When
            var connectionDetails = new ConnectionDetails( "host".AsNonNull( ), "username".AsNonNull( ), "key".AsNonNull( ) );

            // Then
            Assert.NotNull( connectionDetails );
        }
    }
}