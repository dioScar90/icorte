namespace ICorteApi.Domain.Interfaces;

public interface IRecurringScheduleErrors : IBaseErrors<RecurringSchedule>
{
    void ThrowRecurringScheduleNotBelongsToBarberShopException(int barberShopId);
}
