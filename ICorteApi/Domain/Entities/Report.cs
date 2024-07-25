using ICorteApi.Domain.Base;
using ICorteApi.Domain.Enums;
using ICorteApi.Domain.Interfaces;

namespace ICorteApi.Domain.Entities;

public class Report : BaseEntity, IPrimaryKeyEntity<int>
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public Rating Rating { get; set; }

    public int ClientId { get; set; }
    public Person Client { get; set; }

    public int BarberShopId { get; set; }
    public BarberShop BarberShop { get; set; }
    
    public int Key => Id;
}
