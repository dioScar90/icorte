using System.Linq.Expressions;
using ICorteApi.Domain.Interfaces;

namespace ICorteApi.Infraestructure.Interfaces;

public interface IBasePrimaryKeyRepository<TEntity, TKey> : IBaseRepository<TEntity>
    where TEntity : class, IPrimaryKeyEntity<TKey>, IBaseTableEntity
    where TKey : IEquatable<TKey>
{
    Task<ISingleResponse<TEntity>> GetByIdAsync(TKey id, Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[]? includes);
}
