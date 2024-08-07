﻿using ICorteApi.Application.Dtos;
using ICorteApi.Presentation.Extensions;
using ICorteApi.Presentation.Enums;
using FluentValidation;
using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Interfaces;
using ICorteApi.Domain.Entities;

namespace ICorteApi.Presentation.Endpoints;

public static class RecurringScheduleEndpoint
{
    private static readonly string INDEX = "";
    private static readonly string ENDPOINT_PREFIX = EndpointPrefixes.BarberShop + "/{barberShopId}/" + EndpointPrefixes.RecurringSchedule;
    private static readonly string ENDPOINT_NAME = EndpointNames.RecurringSchedule;

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup(ENDPOINT_PREFIX)
            .WithTags(ENDPOINT_NAME)
            .RequireAuthorization();

        group.MapGet(INDEX, GetAllRecurringSchedules);
        group.MapGet("{dayOfWeek}", GetRecurringSchedule);
        group.MapPost(INDEX, CreateRecurringSchedule);
        group.MapPut("{dayOfWeek}", UpdateRecurringSchedule);
        group.MapDelete("{dayOfWeek}", DeleteRecurringSchedule);
    }
    
    public static IResult GetCreatedResult(DayOfWeek newId, int barberShopId)
    {
        string uri = EndpointPrefixes.BarberShop + "/" + barberShopId + "/" + EndpointPrefixes.RecurringSchedule + "/" + newId;
        object value = new { Message = "Horário de funcionamento criado com sucesso" };
        return Results.Created(uri, value);
    }

    public static async Task<IResult> GetRecurringSchedule(
        int barberShopId,
        DayOfWeek dayOfWeek,
        IRecurringScheduleService service,
        IRecurringScheduleErrors errors)
    {
        var res = await service.GetByIdAsync(dayOfWeek, barberShopId);

        if (!res.IsSuccess)
            errors.ThrowNotFoundException();

        var address = res.Value!;

        if (address.BarberShopId != barberShopId)
            errors.ThrowBadRequestException();

        var addressDto = address.CreateDto();
        return Results.Ok(addressDto);
    }

    public static async Task<IResult> GetAllRecurringSchedules(
        int? page,
        int? pageSize,
        int barberShopId,
        IRecurringScheduleService service,
        IRecurringScheduleErrors errors)
    {
        var response = await service.GetAllAsync(page, pageSize);

        if (!response.IsSuccess)
            errors.ThrowNotFoundException();

        var dtos = response.Values!
            .Select(b => b.CreateDto())
            .ToArray();

        return Results.Ok(dtos);
    }

    public static async Task<IResult> CreateRecurringSchedule(
        int barberShopId,
        RecurringScheduleDtoRequest dto,
        IValidator<RecurringScheduleDtoRequest> validator,
        IRecurringScheduleService service,
        IRecurringScheduleErrors errors)
    {
        dto.CheckAndThrowExceptionIfInvalid(validator, errors);

        var response = await service.CreateAsync(dto, barberShopId);

        if (!response.IsSuccess)
            errors.ThrowCreateException();

        return GetCreatedResult(response.Value!.DayOfWeek, barberShopId);
    }

    public static async Task<IResult> UpdateRecurringSchedule(
        int barberShopId,
        DayOfWeek dayOfWeek,
        RecurringScheduleDtoRequest dto,
        IValidator<RecurringScheduleDtoRequest> validator,
        IRecurringScheduleService service,
        IRecurringScheduleErrors errors)
    {
        dto.CheckAndThrowExceptionIfInvalid(validator, errors);

        var response = await service.UpdateAsync(dto, dayOfWeek, barberShopId);

        if (!response.IsSuccess)
            errors.ThrowUpdateException();

        return Results.NoContent();
    }

    public static async Task<IResult> DeleteRecurringSchedule(
        int barberShopId,
        DayOfWeek dayOfWeek,
        IRecurringScheduleService service,
        IRecurringScheduleErrors errors)
    {
        var response = await service.DeleteAsync(dayOfWeek, barberShopId);

        if (!response.IsSuccess)
            errors.ThrowDeleteException();

        return Results.NoContent();
    }
}
