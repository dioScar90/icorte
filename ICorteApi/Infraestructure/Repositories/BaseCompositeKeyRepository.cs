using System.Linq.Expressions;
using ICorteApi.Domain.Base;
using ICorteApi.Domain.Errors;
using ICorteApi.Domain.Interfaces;
using ICorteApi.Infraestructure.Context;
using ICorteApi.Infraestructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ICorteApi.Infraestructure.Repositories;

public abstract class BaseCompositeKeyRepository<TEntity, TKey1, TKey2>(AppDbContext context)
    : BaseRepository<TEntity>(context), IBaseCompositeKeyRepository<TEntity, TKey1, TKey2>
    where TEntity : class, ICompositeKeyEntity<TKey1, TKey2>, IBaseTableEntity
{
    public async Task<ISingleResponse<TEntity>> GetByIdAsync(
        TKey1 id1, TKey2 id2,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (includes is not null && includes.Length > 0)
            foreach (var include in includes)
                query  = query.Include(include);
                
        var entity = await query.SingleOrDefaultAsync(x => x.Id1!.Equals(id1) && x.Id2!.Equals(id2));

        if (entity is null)
            return Response.Failure<TEntity>(Error.TEntityNotFound);
        
        return Response.Success(entity);
    }
}
