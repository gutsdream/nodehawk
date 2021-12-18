using Domain.Entities;
using Domain.ValueObjects;
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
            var awsDetails = new AwsDetails( new NotNullOrWhitespace( accessKey ), new NotNullOrWhitespace( secretKey ) );

            // Then
            Assert.Equal( accessKey, awsDetails.AccessKey );
            Assert.Equal( secretKey, awsDetails.SecretKey );
        }
    }
}