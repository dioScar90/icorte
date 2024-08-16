using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Entities;

namespace ICorteApi.Application.Dtos;

public record ProfileDtoRegisterRequest(
    string FirstName,
    string LastName,
    Gender Gender,
    string PhoneNumber,
    string? ImageUrl = null
) : IDtoRequest<Profile>;

public record ProfileDtoRequest(
    string FirstName,
    string LastName,
    Gender Gender,
    string PhoneNumber,
    string? ImageUrl = null
) : IDtoRequest<Profile>;

public record ProfileDtoResponse(
    int Id,
    string FirstName,
    string LastName,
    Gender Gender,
    string? ImageUrl
) : IDtoResponse<Profile>;