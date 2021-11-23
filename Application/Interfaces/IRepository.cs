using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRepository
    {
        Task Add<TEntity>( TEntity entity ) where TEntity : class;
        Task<List<TEntity>> GetAll<TEntity>( Expression<Func<TEntity, bool>> predicate = null ) where TEntity : class;
        Task<TEntity> GetFirstOrDefault<TEntity>( Expression<Func<TEntity, bool>> predicate = null ) where TEntity : class;
    }
}