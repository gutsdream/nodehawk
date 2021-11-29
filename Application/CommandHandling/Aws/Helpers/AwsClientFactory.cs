using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Application.Interfaces;
using Domain.Entities;

namespace Application.CommandHandling.Aws.Helpers
{
    public static class AwsClientFactory
    {
        public static AmazonS3Client GetS3ClientFromAwsDetails( ICypherService cypherService, AwsDetails awsDetails )
        {
            var credentials = new BasicAWSCredentials( cypherService.Decrypt( awsDetails.AccessKey ),
                cypherService.Decrypt( awsDetails.SecretKey ) );

            return new AmazonS3Client( credentials, RegionEndpoint.USWest1 );
        }
    }
}