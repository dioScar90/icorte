using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Entities;
using ICorteApi.Domain.Interfaces;
using ICorteApi.Infraestructure.Interfaces;
using ICorteApi.Presentation.Extensions;

namespace ICorteApi.Application.Services;

public sealed class SpecialScheduleService(ISpecialScheduleRepository specialScheduleRepository)
    : BaseCompositeKeyService<SpecialSchedule, DateOnly, int>(specialScheduleRepository), ISpecialScheduleService
{
    public override async Task<ISingleResponse<SpecialSchedule>> GetByIdAsync(DateOnly date, int barberShopId)
    {
        return await _repository.GetByIdAsync(x => x.Date == date && x.BarberShopId == barberShopId);
    }
    
    public override async Task<IResponse> UpdateAsync(IDtoRequest<SpecialSchedule> dto, DateOnly date, int barberShopId)
    {
        var resp = await GetByIdAsync(date, barberShopId);

        if (!resp.IsSuccess)
            return resp;
        
        var entity = resp.Value!;
        entity.UpdateEntityByDto(dto);
        
        return await _repository.UpdateAsync(entity);
    }

    public override async Task<IResponse> DeleteAsync(DateOnly date, int barberShopId)
    {
        var resp = await GetByIdAsync(date, barberShopId);

        if (!resp.IsSuccess)
            return resp;
        
        var entity = resp.Value!;
        return await _repository.DeleteAsync(entity);
    }
}
