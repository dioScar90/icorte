using ICorteApi.Application.Dtos;
using ICorteApi.Domain.Entities;
using ICorteApi.Domain.Interfaces;

namespace ICorteApi.Application.Interfaces;

public interface IPersonService
{
       Task<IResponse> CreateAsync(Person person);
       Task<ISingleResponse<Person>> GetByIdAsync(int userId);
       Task<ISingleResponse<BarberShop>> GetMyBarberShopAsync();
       Task<ICollectionResponse<Person>> GetAllAsync(int page, int pageSize);
       Task<IResponse> UpdateAsync(int userId, PersonDtoRequest dto);
       Task<IResponse> DeleteAsync(int userId);
}
