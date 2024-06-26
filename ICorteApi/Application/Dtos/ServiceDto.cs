using ICorteApi.Application.Interfaces;

namespace ICorteApi.Application.Dtos;

public record ServiceDtoRequest(
    string Title,
    string Description,
    decimal Price,
    TimeSpan Duration
) : IDtoRequest;

public record ServiceDtoResponse(
    int Id,
    string Title,
    string Description,
    decimal Price,
    TimeSpan Duration
) : IDtoResponse;
