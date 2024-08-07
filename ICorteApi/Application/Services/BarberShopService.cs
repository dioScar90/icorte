using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Entities;
using ICorteApi.Domain.Interfaces;
using ICorteApi.Infraestructure.Interfaces;
using ICorteApi.Presentation.Extensions;

namespace ICorteApi.Application.Services;

public sealed class BarberShopService(IBarberShopRepository repository)
    : BasePrimaryKeyService<BarberShop, int>(repository), IBarberShopService
{
    public async Task<ISingleResponse<BarberShop>> CreateAsync(IDtoRequest<BarberShop> dto, int ownerId)
    {
        var entity = dto.CreateEntity()!;
        
        entity.OwnerId = ownerId;

        return await CreateByEntityAsync(entity);
    }
}
