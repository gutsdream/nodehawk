using System.Linq;
using Domain.Entities;
using Testing.Shared;
using Xunit;

namespace Domain.Testing.Entities.NodeTests
{
    public class CreateSnapshot
    {
        private readonly Node _node;

        public CreateSnapshot( )
        {
            _node = TestData.Create.Node( );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 50 )]
        [InlineData( 100 )]
        public void Should_CreateSnapshot_When_GivenValidParameters( int spaceUsed )
        {
            // When
            _node.CreateSnapshot( spaceUsed, true );

            // Then
            Assert.Single( _node.Snapshots );

            var snapshot = _node.Snapshots.First( );
            Assert.Equal( spaceUsed, snapshot.SpaceUsedPercentage );
            Assert.True( snapshot.ContainerRunning );
        }

        [Theory]
        [InlineData( -1 )]
        [InlineData( 101 )]
        public void Should_ThrowException_WhenSpaceUsed_IsNotAPercentage( int spaceUsed )
        {
            // When
            var exception = Record.Exception( ( ) => _node.CreateSnapshot( spaceUsed, true ) );

            // Then
            Assert.NotNull( exception );
        }
    }
}