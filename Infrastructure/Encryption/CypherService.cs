using Application.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace Infrastructure.Encryption
{
    public class CypherService : ICypherService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        
        /// <summary>
        /// This is just an identifier used to find a key that's generated on the machine locally, will link to a unique key on each machine it
        /// runs on.
        /// </summary>
        private const string KeyIdentifier = "4240a752-dd6b-4739-9e94-09083f652ef1";

        public CypherService( IDataProtectionProvider dataProtectionProvider )
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt( string value )
        {
            var protector = _dataProtectionProvider.CreateProtector( KeyIdentifier );
            return protector.Protect( value );
        }

        public string Decrypt( string value )
        {
            var protector = _dataProtectionProvider.CreateProtector( KeyIdentifier );
            return protector.Unprotect( value );
        }
    }
}