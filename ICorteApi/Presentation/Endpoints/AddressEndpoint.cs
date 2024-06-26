﻿using Microsoft.EntityFrameworkCore;
using ICorteApi.Infraestructure.Context;
using ICorteApi.Application.Dtos;
using ICorteApi.Domain.Entities;
using ICorteApi.Presentation.Extensions;

namespace ICorteApi.Presentation.Endpoints;

public static class AddressEndpoint
{
    public static void MapAddressEndpoint(this IEndpointRouteBuilder app)
    {
        const string INDEX = "";
        var group = app.MapGroup("address");

        // group.MapGet(INDEX, GetAddresses);
        group.MapGet("{id}", GetAddress);
        group.MapPost(INDEX, CreateAddress);
        group.MapPut("{id}", UpdateAddress);
        group.MapDelete("{id}", DeleteAddress);
    }

    public static async Task<IResult> GetAddress(int id, AppDbContext context)
    {
        var address = await context.Addresses.SingleOrDefaultAsync(a => a.Id == id);

        if (address is null)
            return Results.NotFound("Agendamento não encontrado");

        var addressDto = address.CreateDto<AddressDtoResponse>();
        return Results.Ok(addressDto);
    }

    public static async Task<IResult> CreateAddress(AddressDtoRequest dto, AppDbContext context)
    {
        try
        {
            var newAddress = dto.CreateEntity<Address>()!;

            await context.Addresses.AddAsync(newAddress);
            await context.SaveChangesAsync();

            return Results.Created($"/address/{newAddress.Id}", new { Message = "Endereço criado com sucesso" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> UpdateAddress(int id, AddressDtoRequest dto, AppDbContext context)
    {
        try
        {
            var address = await context.Addresses.SingleOrDefaultAsync(a => a.Id == id);

            if (address is null)
                return Results.NotFound();

            address.Street = dto.Street;
            address.Number = dto.Number;
            address.Complement = dto.Complement;
            address.Neighborhood = dto.Neighborhood;
            address.City = dto.City;
            address.State = dto.State;
            address.PostalCode = dto.PostalCode;
            address.Country = dto.Country;

            address.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return Results.Created($"/address/{address.Id}", new { Message = "Endereço atualizado com sucesso" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> DeleteAddress(int id, AppDbContext context)
    {
        try
        {
            var address = await context.Addresses.SingleOrDefaultAsync(a => a.Id == id);

            if (address is null)
                return Results.NotFound();

            address.UpdatedAt = DateTime.UtcNow;
            address.IsDeleted = true;

            await context.SaveChangesAsync();
            return Results.Created($"/address/{address.Id}", new { Message = "Endereço removido com sucesso" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
