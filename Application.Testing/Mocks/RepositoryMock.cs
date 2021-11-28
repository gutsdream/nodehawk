using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Application.Interfaces;
using Moq;

namespace Application.Testing.Mocks
{
    public class RepositoryMock : Mock<IRepository>
    {
        private List<object> _entities;

        public void WithEntitiesFor<TEntity>( Func<List<TEntity>> invokableEntities ) where TEntity : class
        {
            _entities = new List<object>( );

            Setup( x => x.Get<TEntity>( ) ).Returns( ( ) =>
            {
                var entities = invokableEntities.Invoke( );
                return new EvaluatableMock<TEntity>( entities ).Object;
            } );

            Setup( x => x.Add( It.IsAny<TEntity>( ) ) ).Callback( ( TEntity entity ) => { _entities.Add( entity ); } );

            Setup( x => x.Remove( It.IsAny<TEntity>( ) ) ).Callback( ( TEntity entity ) => { _entities.Remove( entity ); } );
        }

        public bool Contains<TEntity>( Func<TEntity, bool> predicate ) where TEntity : class
        {
            return _entities.OfType<TEntity>( ).Any( predicate );
        }

        public bool DoesNotContain<TEntity>( Func<TEntity, bool> predicate ) where TEntity : class
        {
            return !_entities.OfType<TEntity>( ).Any( predicate );
        }
    }

    public class EvaluatableMock<TEntity> : Mock<IEvaluatable<TEntity>> where TEntity : class
    {
        private readonly List<TEntity> _entities;

        public EvaluatableMock( List<TEntity> entities )
        {
            _entities = entities;

            Setup( x => x.Where( It.IsAny<Expression<Func<TEntity, bool>>>( ) ) ).Returns( ( Expression<Func<TEntity, bool>> func ) =>
            {
                var filteredEntities = _entities.Where( func.Compile( ) ).ToList( );
                return new EvaluatableMock<TEntity>( filteredEntities ).Object;
            } );

            Setup( x => x.Include( It.IsAny<Expression<Func<TEntity, It.IsAnyType>>>( ) ) ).Returns( new InvocationFunc( invocation =>
            {
                return new EvaluatableMock<TEntity>( _entities ).Object;
            }) );

            Setup( x => x.AnyAsync( It.IsAny<Expression<Func<TEntity, bool>>>( ) ) ).Returns( async ( Expression<Func<TEntity, bool>> func ) =>
            {
                return _entities.Any( func.Compile( ) );
            } );

            Setup( x => x.FirstOrDefaultAsync( It.IsAny<Expression<Func<TEntity, bool>>>( ) ) )
                .Returns( async ( Expression<Func<TEntity, bool>> func ) => { return _entities.FirstOrDefault( func.Compile( ) ); } );

            Setup( x => x.FirstAsync( It.IsAny<Expression<Func<TEntity, bool>>>( ) ) ).Returns( async ( Expression<Func<TEntity, bool>> func ) =>
            {
                return _entities.First( func.Compile( ) );
            } );

            Setup( x => x.ToListAsync( ) ).Returns( async ( ) => { return _entities.ToList( ); } );
        }
    }
}