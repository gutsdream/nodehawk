using Domain.ValueObjects;

namespace Domain.Entities
{
    public class AwsDetails : Entity
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }

        protected AwsDetails( )
        {
            
        }
        
        public AwsDetails( NotNullOrWhitespace accessKey, NotNullOrWhitespace secretKey )
        {
            UpdateAccessKey( accessKey );
            UpdateSecretKey( secretKey );
        }

        public void UpdateAccessKey( NotNullOrWhitespace accessKey )
        {
            AccessKey = accessKey.Value;
        }
        
        public void UpdateSecretKey( NotNullOrWhitespace secretKey )
        {
            SecretKey = secretKey.Value;
        }
    }
}