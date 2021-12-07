using Application.Core.Interfaces;
using Moq;

namespace Application.Testing.Mocks
{
    public class NodeHawkSshClientMock : Mock<INodeHawkSshClient>
    {
        public NodeHawkSshClientMock( )
        {
            Setup( x => x.AreConnectionDetailsValid( It.IsAny<string>( ), It.IsAny<string>( ), It.IsAny<string>( ) ) ).Returns( true );
        }

        public void SshAuthenticationShouldFail( )
        {
            Setup( x => x.AreConnectionDetailsValid( It.IsAny<string>( ), It.IsAny<string>( ), It.IsAny<string>( ) ) ).Returns( false );
        }
    }
}