using System.Globalization;
using ICorteApi.Domain.Entities;
using ICorteApi.Infraestructure.Context;
using ICorteApi.Infraestructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ICorteApi.Infraestructure.Repositories;

public sealed class BarberScheduleRepository(AppDbContext context) : IBarberScheduleRepository
{
    private readonly AppDbContext _context = context;
    private readonly DbSet<Service> _dbSetService = context.Set<Service>();
    private readonly DbSet<Appointment> _dbSetAppointment = context.Set<Appointment>();

    private async Task<TimeSpan> CalculateTotalServiceDuration(int[] serviceIds)
    {
        var timeSpans = await _dbSetService
            .AsNoTracking()
            .Where(x => serviceIds.Contains(x.Id))
            .Select(x => new { x.Duration })
            .ToArrayAsync();
        
        return timeSpans.Aggregate(new TimeSpan(0), (acc, curr) => acc.Add(curr.Duration));
    }

    private async Task<Appointment[]> GetAppointmentsByDateAsync(int barberShopId, DateOnly date)
    {
        return await _dbSetAppointment
            .AsNoTracking()
            .Where(x => x.BarberShopId == barberShopId && x.Date == date)
            .OrderBy(x => x.StartTime)
            .ToArrayAsync();
    }

    private static TimeOnly[] CalculateAvailableSlots(TimeOnly openTime, TimeOnly closeTime, Appointment[] appointments, TimeSpan serviceDuration)
    {
        var availableSlots = new List<TimeOnly>();
        var currentTime = openTime;
        
        foreach (var appointment in appointments)
        {
            var nextAppointmentStartTime = appointment.StartTime;
            
            if (nextAppointmentStartTime > currentTime)
            {
                var availableDuration = nextAppointmentStartTime - currentTime;
                
                if (availableDuration >= serviceDuration)
                    availableSlots.Add(currentTime);
            }
            
            currentTime = appointment.StartTime.Add(appointment.Services.Max(s => s.Duration));
        }
        
        if (closeTime > currentTime)
        {
            var availableDuration = closeTime - currentTime;

            if (availableDuration >= serviceDuration)
                availableSlots.Add(currentTime);
        }

        return [..availableSlots];
    }

    public async Task<TimeOnly[]> GetAvailableSlotsAsync(int barberShopId, DateOnly date, int[] serviceIds)
    {
        var schedule = await _context.Database
            .SqlQuery<AvailableSchedule>(@$"
                SELECT TOP 1 {date} AS Date
                    ,COALESCE(SS.open_time, RS.open_time) AS OpenTime
                    ,COALESCE(SS.close_time, RS.close_time) AS CloseTime
                FROM recurring_schedules AS RS
                    LEFT JOIN special_schedules AS SS
                        ON SS.barber_shop_id = RS.barber_shop_id
                            AND RS.day_of_week = DATEPART(weekday, SS.date) - 1
                            AND SS.is_active = 1
                WHERE RS.is_active = 1
                    AND RS.barber_shop_id = {barberShopId}
                    AND RS.day_of_week = {date.DayOfWeek}
                    AND (SS.is_closed IS NULL OR SS.is_closed = 0)
            ")
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (schedule is null)
            return [];
            
        var totalDuration = await CalculateTotalServiceDuration(serviceIds);
        var appointments = await GetAppointmentsByDateAsync(barberShopId, date);

        return CalculateAvailableSlots(schedule.OpenTime, schedule.CloseTime, appointments, totalDuration);
    }
    
    public async Task<BarberShop[]> GetTopBarbersWithAvailabilityAsync(int weekNumber, int take)
    {
        DateOnly firstDayOfWeek = DateOnly.FromDateTime(
            ISOWeek.ToDateTime(DateTime.Now.Year, weekNumber, DayOfWeek.Sunday)
        );
        
        DateOnly lastDayOfWeek = firstDayOfWeek.AddDays(6);
        
        return await _context.Database
            .SqlQuery<BarberShop>(@$"
                WITH available_days AS (
                    SELECT 
                        RS.barber_shop_id,
                        DATEADD(DAY, RS.day_of_week, {firstDayOfWeek}) AS available_date
                    FROM recurring_schedules AS RS
                    LEFT JOIN special_schedules AS SS
                        ON SS.barber_shop_id = RS.barber_shop_id
                        AND SS.is_active = 1
                        AND DATEADD(DAY, RS.day_of_week, {firstDayOfWeek}) = SS.date
                    WHERE RS.is_active = 1
                    AND (SS.is_closed IS NULL OR SS.is_closed = 0)
                    AND DATEADD(DAY, RS.day_of_week, {firstDayOfWeek}) BETWEEN {firstDayOfWeek} AND {lastDayOfWeek}
                    GROUP BY RS.barber_shop_id, RS.day_of_week, SS.open_time, SS.close_time
                )

                SELECT TOP {take} BS.*
                FROM barber_shops AS BS
                WHERE EXISTS (
                    SELECT 1
                    FROM available_days AS AD
                    WHERE AD.barber_shop_id = BS.id
                )
                ORDER BY BS.rating DESC
            ")
            .AsNoTracking()
            .ToArrayAsync();
    }

    
    public async Task<DateOnly[]> GetAvailableDaysForBarberAsync(int barberShopId, DateOnly randomDate)
    {
        DateOnly sunday = randomDate.AddDays(-(int)randomDate.DayOfWeek);
        
        return await _context.Database
            .SqlQuery<AvailableSchedule>(@$"
                SELECT DATEADD(DAY, RS.day_of_week, {sunday}) AS Date
                    ,COALESCE(SS.open_time, RS.open_time) AS OpenTime
                    ,COALESCE(SS.close_time, RS.close_time) AS CloseTime
                FROM recurring_schedules AS RS
                    LEFT JOIN special_schedules AS SS
                        ON SS.barber_shop_id = RS.barber_shop_id
                            AND SS.is_active = 1
                            AND DATEADD(DAY, RS.day_of_week, {sunday}) = SS.date
                WHERE RS.is_active = 1
                    AND RS.barber_shop_id = {barberShopId}
                    AND (SS.is_closed IS NULL OR SS.is_closed = 0)
                ORDER BY 1
            ")
            .AsNoTracking()
            .Select(x => x.Date)
            .ToArrayAsync();
    }
    
    private record AvailableSchedule(
        DateOnly Date,
        TimeOnly OpenTime,
        TimeOnly CloseTime
    );
}
