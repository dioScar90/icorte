using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Entities;

namespace ICorteApi.Application.Dtos;

public record RecurringScheduleDtoRequest(
    DayOfWeek DayOfWeek,
    int? BarberShopId,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsActive
) : IDtoRequest<RecurringSchedule>;

public record RecurringScheduleDtoResponse(
    DayOfWeek DayOfWeek,
    int BarberShopId,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsActive
) : IDtoResponse<RecurringSchedule>;
