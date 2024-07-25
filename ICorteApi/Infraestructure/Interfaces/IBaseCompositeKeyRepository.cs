using System.Linq.Expressions;
using ICorteApi.Domain.Interfaces;

namespace ICorteApi.Infraestructure.Interfaces;

public interface IBaseCompositeKeyRepository<TEntity, TKey1, TKey2> : IBaseRepository<TEntity>
    where TEntity : class, ICompositeKeyEntity<TKey1, TKey2>, IBaseTableEntity
{
    Task<ISingleResponse<TEntity>> GetByIdAsync(TKey1 key1, TKey2 key2, params Expression<Func<TEntity, object>>[]? includes);
}
