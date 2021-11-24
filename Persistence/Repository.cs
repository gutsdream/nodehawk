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

        public IEvaluatable<TEntity> Get<TEntity>( ) where TEntity : class
        {
            return new Evaluatable<TEntity>( SetFor<TEntity>( ) );
        }

        public async Task Save( )
        {
            await _context.SaveChangesAsync( );
        }
    }

    public class Evaluatable<TEntity> : IEvaluatable<TEntity> where TEntity : class
    {
        private readonly IQueryable<TEntity> _queryable;

        public Evaluatable( IQueryable<TEntity> queryable )
        {
            _queryable = queryable;
        }

        public IEvaluatable<TEntity> Where( Expression<Func<TEntity, bool>> predicate )
        {
            return new Evaluatable<TEntity>( _queryable.Where( predicate ) );
        }

        public IEvaluatable<TEntity> Include<TProperty>( Expression<Func<TEntity, TProperty>> include )
        {
            return new Evaluatable<TEntity>( _queryable.Include( include ) );
        }

        public async Task<bool> AnyAsync( Expression<Func<TEntity, bool>> predicate = null )
        {
            return predicate != null
                ? await _queryable.AnyAsync( predicate )
                : await _queryable.AnyAsync( );
        }

        public async Task<TEntity> FirstAsync( Expression<Func<TEntity, bool>> predicate = null )
        {
            return predicate != null
                ? await _queryable.FirstAsync( predicate )
                : await _queryable.FirstAsync( );
        }

        public async Task<TEntity> FirstOrDefaultAsync( Expression<Func<TEntity, bool>> predicate = null )
        {
            return predicate != null
                ? await _queryable.FirstOrDefaultAsync( predicate )
                : await _queryable.FirstOrDefaultAsync( );
        }

        public async Task<List<TEntity>> ToListAsync( )
        {
            return await _queryable.ToListAsync( );
        }
    }
}