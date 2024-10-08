namespace ICorteApi.Application.Dtos;

public record AddressDtoCreate(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    State State,
    string PostalCode,
    string Country
) : IDtoRequest<Address>;

public record AddressDtoUpdate(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    State State,
    string PostalCode,
    string Country
) : IDtoRequest<Address>;

public record AddressDtoResponse(
    int Id,
    int BarberShopId,
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    State State,
    string PostalCode,
    string Country
) : IDtoResponse<Address>;
