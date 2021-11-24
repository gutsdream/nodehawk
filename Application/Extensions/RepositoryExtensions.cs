using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<bool> Exists<TEntity>( this IRepository repository, Expression<Func<TEntity, bool>> predicate )
            where TEntity : class
        {
            return await repository.Get<TEntity>()
                .AnyAsync( predicate );
        }
    }
}