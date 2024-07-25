using ICorteApi.Domain.Entities;
using ICorteApi.Infraestructure.Context;
using ICorteApi.Infraestructure.Interfaces;

namespace ICorteApi.Infraestructure.Repositories;

public class AddressRepository(AppDbContext context)
    : BasePrimaryKeyRepository<Address, int>(context), IAddressRepository
{
}
