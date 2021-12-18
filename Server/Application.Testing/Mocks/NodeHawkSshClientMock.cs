using Application.Core.Interfaces;
using Infrastructure.Ssh;
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

        public void CommandShouldReturn( string command, string contentToReturn )
        {
            Setup( x => x.Run( It.Is<SshMessage>( y => y.Value == command ) ) ).Returns( new SshCommandResult( contentToReturn ) );
        }
    }
}