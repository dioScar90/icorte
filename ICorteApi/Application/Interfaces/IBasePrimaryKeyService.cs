using ICorteApi.Domain.Interfaces;

namespace ICorteApi.Application.Interfaces;

public interface IBasePrimaryKeyService<TEntity, TKey> : IBaseService<TEntity>
    where TEntity : class, IPrimaryKeyEntity<TKey>, IBaseTableEntity
    where TKey : IEquatable<TKey>
{
    Task<ISingleResponse<TEntity>> GetByIdAsync(TKey id);
    Task<IResponse> UpdateAsync(IDtoRequest<TEntity> dto, TKey id);
    Task<IResponse> DeleteAsync(TKey id);
}
