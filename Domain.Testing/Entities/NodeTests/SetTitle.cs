using Domain.Entities;
using Xunit;

namespace Domain.Testing.Entities.NodeTests
{
    public class SetTitle
    {
        private readonly Node _node;

        public SetTitle( )
        {
            _node = TestData.Create.Node( );
        }

        [Fact]
        public void Should_SetTitle_When_TitleNotNull( )
        {
            // Given
            const string newTitle = "New Fancy Title";

            // When
            _node.SetTitle( newTitle );

            // Then
            Assert.Equal( newTitle, _node.Title );
        }

        [Fact]
        public void Should_ThrowException_When_TitleNull( )
        {
            // When
            var ex = Record.Exception( ( ) => _node.SetTitle( null ) );

            // Then
            Assert.NotNull( ex );
        }
    }
}