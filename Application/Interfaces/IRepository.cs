using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRepository
    {
        Task AddAsync<TEntity>( TEntity entity ) where TEntity : class;
        IEvaluatable<TEntity> Get<TEntity>( ) where TEntity : class;
        void Remove<TEntity>( TEntity entity ) where TEntity : class;
        Task SaveAsync( );
    }

    /// <summary>
    /// Obvious IQueryable abstraction is obvious
    /// </summary>
    public interface IEvaluatable<TEntity> where TEntity : class
    {
        IEvaluatable<TEntity> Where( Expression<Func<TEntity, bool>> predicate );
        IEvaluatable<TEntity> Include<TProperty>( Expression<Func<TEntity, TProperty>> include );
        
        Task<bool> AnyAsync( Expression<Func<TEntity, bool>> predicate = null );

        Task<TEntity> FirstAsync( Expression<Func<TEntity, bool>> predicate = null );
        Task<TEntity> FirstOrDefaultAsync( Expression<Func<TEntity, bool>> predicate = null );

        Task<List<TEntity>> ToListAsync( );
    }
}