using Domain.ExceptionHandling;

namespace Domain.Entities
{
    public class AwsDetails : Entity
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }

        public AwsDetails( string accessKey, string secretKey )
        {
            UpdateAccessKey( accessKey );
            UpdateSecretKey( secretKey );
        }

        public void UpdateSecretKey( string secretKey )
        {
            Throw.If.NullOrWhitespace( secretKey, nameof( secretKey ) );
            SecretKey = secretKey;
        }

        public void UpdateAccessKey( string accessKey )
        {
            Throw.If.NullOrWhitespace( accessKey, nameof( accessKey ) );
            AccessKey = accessKey;
        }
    }
}