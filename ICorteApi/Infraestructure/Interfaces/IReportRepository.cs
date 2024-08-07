using ICorteApi.Domain.Entities;

namespace ICorteApi.Infraestructure.Interfaces;

public interface IReportRepository
    : IBasePrimaryKeyRepository<Report, int>
{
}
