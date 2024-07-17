using ICorteApi.Domain.Base;
using ICorteApi.Domain.Enums;

namespace ICorteApi.Domain.Entities;

// Comentário/Relato
public class Report : BaseEntity
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public Rating Rating { get; set; }
    
    public int ClientId { get; set; }
    public Person Client { get; set; }

    public int BarberShopId { get; set; }
    public BarberShop BarberShop { get; set; }
}
