using ICorteApi.Domain.Base;
using ICorteApi.Domain.Interfaces;

namespace ICorteApi.Domain.Entities;

public class SpecialSchedule : CompositeKeyEntity<DateOnly, int>
{
    // public DateOnly Date => Id1;
    public DateOnly Date { get => Id1; set => Id1 = value; }
    public string? Notes { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public bool IsClosed { get; set; } = false;

    // public int BarberShopId => Id2;
    public int BarberShopId { get => Id2; set => Id2 = value; }
    public BarberShop BarberShop { get; set; }
    
    // public DateOnly Key1 => Date;
    // public int Key2 => BarberShopId;
}
