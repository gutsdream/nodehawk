using Application.Core.Interfaces;
using Moq;

namespace Application.Testing.Mocks
{
    public class CypherServiceMock : Mock<ICypherService>
    {
        public CypherServiceMock( )
        {
            Setup( x => x.Encrypt( It.IsAny<string>( ) ) ).Returns( ( string value ) => value );
            Setup( x => x.Decrypt( It.IsAny<string>( ) ) ).Returns( ( string value ) => value );
        }
    }
}