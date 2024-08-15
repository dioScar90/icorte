using ICorteApi.Application.Dtos;
using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Entities;
using ICorteApi.Domain.Enums;
using ICorteApi.Domain.Interfaces;
using ICorteApi.Infraestructure.Interfaces;

namespace ICorteApi.Application.Services;

public sealed class BarberShopService(IBarberShopRepository repository, IUserRepository userRepository)
    : BasePrimaryKeyService<BarberShop, int>(repository), IBarberShopService
{
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<ISingleResponse<BarberShop>> CreateAsync(IDtoRequest<BarberShop> dtoRequest, int ownerId)
    {
        if (dtoRequest is not BarberShopDtoRequest dto)
            throw new ArgumentException("Tipo de DTO inválido", nameof(dtoRequest));

        var entity = new BarberShop(dto, ownerId);
        var result = await CreateByEntityAsync(entity);

        if (!result.IsSuccess)
            return result;

        await _userRepository.AddUserRoleAsync(UserRole.BarberShop);
        return result;
    }

    public override async Task<IResponse> DeleteAsync(int id)
    {
        var resp = await GetByIdAsync(id);

        if (!resp.IsSuccess)
            return resp;
        
        await _userRepository.RemoveFromRoleAsync(UserRole.BarberShop);
        return await _repository.DeleteAsync(resp.Value!);
    }
}