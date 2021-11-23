using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class Repository : IRepository
    {
        private readonly DataContext _context;

        private DbSet<TEntity> SetFor<TEntity>( ) where TEntity : class => _context.Set<TEntity>( );

        public Repository( DataContext context )
        {
            _context = context;
        }

        public async Task Add<TEntity>( TEntity entity ) where TEntity : class
        {
            await SetFor<TEntity>( ).AddAsync( entity );
        }

        public async Task<List<TEntity>> GetAll<TEntity>( Expression<Func<TEntity, bool>> predicate = null )
            where TEntity : class
        {
            return await SetFor<TEntity>( )
                .WhereIf( predicate != null, predicate )
                .ToListAsync( );
        }

        public async Task<TEntity> GetFirstOrDefault<TEntity>( Expression<Func<TEntity, bool>> predicate = null )
            where TEntity : class
        {
            return await SetFor<TEntity>( )
                .WhereIf( predicate != null, predicate )
                .FirstOrDefaultAsync( );
        }
    }

    internal static class QueryableExtensions
    {
        public static IQueryable<TEntity> WhereIf<TEntity>( this IQueryable<TEntity> query,
            bool condition,
            Expression<Func<TEntity, bool>> predicate )
        {
            return condition
                ? query.Where( predicate )
                : query;
        }
    }
}