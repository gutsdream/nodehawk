using Domain.Entities;
using Xunit;

namespace Domain.Testing.Entities.ConnectionDetailsTests
{
    public class ConnectionDetailsConstructor
    {
        [Fact]
        public void Should_SuccessfullyConstructConnectionDetails_When_GivenValidParameters( )
        {
            // When
            var connectionDetails = new ConnectionDetails( "host", "username", "key" );

            // Then
            Assert.NotNull( connectionDetails );
        }

        [Fact]
        public void Should_ThrowException_When_HostIsNull( )
        {
            // When
            var exception = Record.Exception( ( ) => new ConnectionDetails( null, "username", "key" ) );

            // Then
            Assert.NotNull( exception );
        }

        [Fact]
        public void Should_ThrowException_When_UsernameIsNull( )
        {
            // When
            var exception = Record.Exception( ( ) => new ConnectionDetails( "host", null, "key" ) );

            // Then
            Assert.NotNull( exception );
        }

        [Fact]
        public void Should_ThrowException_When_KeyIsNull( )
        {
            // When
            var exception = Record.Exception( ( ) => new ConnectionDetails( "host", "username", null ) );

            // Then
            Assert.NotNull( exception );
        }
    }
}