using ICorteApi.Domain.Entities;
using ICorteApi.Infraestructure.Context;
using ICorteApi.Infraestructure.Interfaces;

namespace ICorteApi.Infraestructure.Repositories;

public sealed class ReportRepository(AppDbContext context)
    : BasePrimaryKeyRepository<Report, int>(context), IReportRepository
{
}
