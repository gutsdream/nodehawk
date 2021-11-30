using Domain.Entities;
using Xunit;

namespace Domain.Testing.Entities.AwsDetailsTests
{
    public class AwsDetailsConstructorTests
    {
        [Fact]
        public void Should_ConstructAwsDetails_When_GivenValidParameters( )
        {
            // Given
            var accessKey = "accessKey";
            var secretKey = "secretKey";

            // When
            var awsDetails = new AwsDetails( accessKey, secretKey );

            // Then
            Assert.Equal( accessKey, awsDetails.AccessKey );
            Assert.Equal( secretKey, awsDetails.SecretKey );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        [InlineData( "     " )]
        public void Should_ThrowException_When_AccessKeyIsNullOrEmptyOrWhitespace( string accessKey )
        {
            // Given
            var secretKey = "secretKey";

            // When
            var exception = Record.Exception( ( ) => new AwsDetails( accessKey, secretKey ) );

            // Then
            Assert.NotNull( exception );
        }
        
        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        [InlineData( "    " )]
        public void Should_ThrowException_When_SecretKeyIsNullOrEmptyOrWhitespace( string secretKey )
        {
            // Given
            var accessKey = "accessKey";

            // When
            var exception = Record.Exception( ( ) => new AwsDetails( accessKey, secretKey ) );

            // Then
            Assert.NotNull( exception );
        }
    }
}