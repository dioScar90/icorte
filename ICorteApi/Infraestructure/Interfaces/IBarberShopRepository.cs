using System.Linq.Expressions;
using ICorteApi.Application.Dtos;
using ICorteApi.Domain.Entities;
using ICorteApi.Domain.Interfaces;

namespace ICorteApi.Infraestructure.Interfaces;

public interface IBarberShopRepository
{
    Task<IResponseModel> CreateAsync(BarberShop barberShop);
    Task<IResponseDataModel<BarberShop>> GetByIdAsync(Expression<Func<BarberShop, bool>>? filter);
    Task<IResponseDataModel<IEnumerable<BarberShop>>> GetAllAsync(Expression<Func<BarberShop, bool>>? filter = null);
    Task<IResponseModel> UpdateAsync(int id, BarberShopDtoRequest dto);
    Task<IResponseModel> DeleteAsync(int id);
}
