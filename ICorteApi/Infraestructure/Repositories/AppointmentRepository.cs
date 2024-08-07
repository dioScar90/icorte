using ICorteApi.Domain.Entities;
using ICorteApi.Infraestructure.Context;
using ICorteApi.Infraestructure.Interfaces;

namespace ICorteApi.Infraestructure.Repositories;

public sealed class AppointmentRepository(AppDbContext context)
    : BasePrimaryKeyRepository<Appointment, int>(context), IAppointmentRepository
{
}
