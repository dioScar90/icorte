using ICorteApi.Application.Dtos;
using ICorteApi.Domain.Entities;

namespace ICorteApi.Presentation.Extensions;

public static class DtoCreator
{
    public static AddressDtoResponse CreateDto(this Address address) =>
        new(
            address.Id,
            address.Street,
            address.Number,
            address.Complement,
            address.Neighborhood,
            address.City,
            address.State,
            address.PostalCode,
            address.Country
        );
        
    public static AppointmentDtoResponse CreateDto(this Appointment appointment) =>
        new(
            appointment.Id,
            appointment.Date,
            appointment.StartTime,
            appointment.EndTime,
            appointment.Notes,
            appointment.TotalPrice,
            appointment.Status
        );
        
    public static BarberShopDtoResponse CreateDto(this BarberShop barberShop) =>
        new(
            barberShop.Id,
            barberShop.Name,
            barberShop.Description,
            barberShop.ComercialNumber,
            barberShop.ComercialEmail,
            barberShop.Address?.CreateDto(),
            barberShop.RecurringSchedules?.Select(b => b.CreateDto()).ToArray(),
            barberShop.Barbers?.Select(b => b.CreateDto()).ToArray()
        );
        
    public static MessageDtoResponse CreateDto(this Message message) =>
        new(
            message.Id,
            message.Content,
            message.SentAt,
            message.IsRead,
            message.Sender.Id,
            message.Sender.FirstName,
            message.Sender.LastName
        );
        
    public static PaymentDtoResponse CreateDto(this Payment payment) =>
        new(
            payment.Id,
            payment.PaymentType,
            payment.Amount
        );
    
    public static RecurringScheduleDtoResponse CreateDto(this RecurringSchedule recurringSchedule) =>
        new(
            recurringSchedule.DayOfWeek,
            recurringSchedule.BarberShopId,
            recurringSchedule.OpenTime,
            recurringSchedule.CloseTime,
            recurringSchedule.IsActive
        );

    public static ReportDtoResponse CreateDto(this Report report) =>
        new(
            report.Id,
            report.Title,
            report.Content,
            report.Rating
        );
        
    public static ServiceDtoResponse CreateDto(this Service service) =>
        new(
            service.Id,
            service.Name,
            service.Description ?? default,
            service.Price,
            service.Duration
        );
        
    public static SpecialScheduleDtoResponse CreateDto(this SpecialSchedule specialSchedule) =>
        new(
            specialSchedule.Date,
            specialSchedule.BarberShopId,
            specialSchedule.Notes,
            specialSchedule.OpenTime,
            specialSchedule.CloseTime,
            specialSchedule.IsClosed
        );

    private static bool CheckUserRegisterCompletation(User user) =>
        user is {
            Email: not null,
            FirstName: not null,
            LastName: not null,
            PhoneNumber: not null,
        };

    public static UserDtoResponse CreateDto(this User user) =>
        new(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.ImageUrl,
            [],
            CheckUserRegisterCompletation(user)
        );
}
