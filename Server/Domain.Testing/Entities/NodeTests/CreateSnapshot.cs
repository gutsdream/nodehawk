using System.Linq;
using Domain.Entities;
using Domain.ValueObjects;
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

        [Fact]
        public void Should_CreateSnapshot( )
        {
            // Given
            var spaceUsed = 50;
            
            // When
            _node.CreateSnapshot( new Percentage( spaceUsed ), true );

            // Then
            Assert.Single( _node.Snapshots );

            var snapshot = _node.Snapshots.First( );
            Assert.Equal( spaceUsed, snapshot.SpaceUsedPercentage );
            Assert.True( snapshot.ContainerRunning );
        }
    }
}