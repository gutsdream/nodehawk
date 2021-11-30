using Domain.Entities;
using Xunit;

namespace Domain.Testing.Entities.AwsDetailsTests
{
    public class UpdateAccessKey
    {
        private readonly AwsDetails _awsDetails;

        public UpdateAccessKey( )
        {
            _awsDetails = new AwsDetails( "supersecret", "supersupersecret" );
        }

        [Fact]
        public void Should_UpdateAccessKey( )
        {
            // Given
            const string accessKey = "secretkey";

            // When
            _awsDetails.UpdateAccessKey( accessKey );

            // Then
            Assert.Equal( accessKey, _awsDetails.AccessKey );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        [InlineData( "     " )]
        public void Should_ThrowException_When_AccessKeyIsNullOrEmptyOrWhitespace( string accessKey )
        {
            // When
            var exception = Record.Exception( ( ) => _awsDetails.UpdateAccessKey( accessKey ) );

            // Then
            Assert.NotNull( exception );
        }
    }
}