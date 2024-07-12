using ICorteApi.Domain.Base;

namespace ICorteApi.Domain.Entities;

// Agenda
public class Schedule : BaseEntity
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } // Indica se o horário está disponível

    public int BarberShopId { get; set; }
    public BarberShop BarberShop { get; set; }

    public int? BarberId { get; set; }
    public Person Barber { get; set; }

    public Appointment Appointment { get; set; }
}
