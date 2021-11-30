using Domain.Entities;
using Xunit;

namespace Domain.Testing.Entities.AwsDetailsTests
{
    public class UpdateSecretKey
    {
        private readonly AwsDetails _awsDetails;

        public UpdateSecretKey( )
        {
            _awsDetails = new AwsDetails( "supersecret", "supersupersecret" );
        }

        [Fact]
        public void Should_UpdateSecretKey( )
        {
            // Given
            const string secretKey = "secretkey";

            // When
            _awsDetails.UpdateSecretKey( secretKey );

            // Then
            Assert.Equal( secretKey, _awsDetails.SecretKey );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        [InlineData( "     " )]
        public void Should_ThrowException_When_SecretKeyIsNullOrEmptyOrWhitespace( string secretKey )
        {
            // When
            var exception = Record.Exception( ( ) => _awsDetails.UpdateSecretKey( secretKey ) );

            // Then
            Assert.NotNull( exception );
        }
    }
}