using System.Security.Policy;
using ICorteApi.Application.Dtos;
using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Base;
using ICorteApi.Domain.Enums;

namespace ICorteApi.Domain.Entities;

public sealed class Appointment : BaseEntity<Appointment>
{
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public string? Notes { get; private set; }
    public AppointmentStatus? Status { get; private set; }

    public int ClientId { get; init; }
    public User Client { get; set; }

    public int BarberShopId { get; init; }
    public BarberShop BarberShop { get; set; }

    public ICollection<Message> Messages { get; init; } = [];
    public ICollection<Payment> Payments { get; init; } = [];
    public ICollection<Service> Services { get; init; } = [];

    private Appointment() { }

    public Appointment(AppointmentDtoRequest dto, Service[] services, int clientId)
    {
        Date = dto.Date;
        StartTime = dto.StartTime;
        Notes = dto.Notes;
        
        Services = services;

        Status = AppointmentStatus.Pending;

        ClientId = clientId;
        BarberShopId = services[0].BarberShopId;
    }

    public void AddServices(Service[] servicesToAdd)
    {
        foreach (var toAdd in servicesToAdd)
            Services.Add(toAdd);
    }
    
    public void RemoveServicesByIds(int[] serviceIdsToRemove)
    {
        HashSet<int> idsToRemove = [..serviceIdsToRemove];
        var servicesToRemove = Services.Where(t => idsToRemove.Contains(t.Id)).ToArray();

        foreach (var toRemove in servicesToRemove)
            Services.Remove(toRemove);
    }
    
    private void UpdateByAppointmentDto(AppointmentDtoRequest dto, DateTime? utcNow)
    {
        utcNow ??= DateTime.UtcNow;

        Date = dto.Date;
        StartTime = dto.StartTime;
        Notes = dto.Notes;
        
        UpdatedAt = utcNow;
    }

    public override void UpdateEntityByDto(IDtoRequest<Appointment> requestDto, DateTime? utcNow = null)
    {
        switch (requestDto)
        {
            case AppointmentDtoRequest dto:
                UpdateByAppointmentDto(dto, utcNow);
                break;
            default:
                throw new ArgumentException("Tipo de DTO inválido", nameof(requestDto));
        }
    }

    private TimeOnly GetTotalDuration() => Services.Aggregate(StartTime, (acc, curr) => acc.Add(curr.Duration));
    private decimal GetTotalPrice() => Services.Aggregate(0M, (acc, curr) => acc + curr.Price);
    private ServiceDtoResponse[] GetAllServices() => [..Services.Select(s => s.CreateDto())];

    public override AppointmentDtoResponse CreateDto() =>
        new(
            Id,
            Date,
            StartTime,
            GetTotalDuration(),
            Notes,
            GetTotalPrice(),
            GetAllServices(),
            Status ?? AppointmentStatus.Pending
        );
}
