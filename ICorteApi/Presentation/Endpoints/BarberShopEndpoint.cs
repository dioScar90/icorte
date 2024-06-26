﻿using Microsoft.EntityFrameworkCore;
using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Entities;
using ICorteApi.Application.Dtos;
using ICorteApi.Infraestructure.Context;
using ICorteApi.Presentation.Extensions;

namespace ICorteApi.Presentation.Endpoints;

public static class BarberShopEndpoint
{
    public static void MapBarberShopEndpoint(this IEndpointRouteBuilder app)
    {
        const string INDEX = "";
        var group = app.MapGroup("barberShop");

        // group.MapGet(INDEX, GetAllBarbers);
        group.MapGet("{id}", GetBarberShop);
        group.MapPost(INDEX, CreateBarberShop);
        group.MapPut("{id}", UpdateBarberShop);
        group.MapDelete("{id}", DeleteBarberShop);
    }

    public static async Task<IResult> GetBarberShop(int id, IBarberShopService barberShopService)
    {
        var barberShop = await barberShopService.GetByIdAsync(id);

        if (!barberShop.Success)
            return Results.NotFound();

        var barberShopDto = barberShop.Data.CreateDto<BarberShopDtoResponse>();
        return Results.Ok(barberShopDto);
    }

    // public static async Task<IResult> GetAllBarbers(int page, int perPage, AppDbContext context)
    // {
    //     var barbers = new BarberRepository(context)
    //         .GetAll(page, perPage);

    //     if (!barbers.Any())
    //         Results.NotFound();

    //     var dtos = await barbers
    //         .Select(b => BarberToDtoResponse(b))
    //         .ToListAsync();

    //     return Results.Ok(dtos);
    // }

    public static async Task<IResult> CreateBarberShop(BarberShopDtoRequest dto, AppDbContext context)
    {
        try
        {
            var newBarberShop = dto.CreateEntity<BarberShop>();

            await context.BarberShops.AddAsync(newBarberShop!);
            await context.SaveChangesAsync();

            return Results.Created($"/barber/{newBarberShop.Id}", new { Message = "Barbearia criada com sucesso" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> UpdateBarberShop(int id, BarberShopDtoRequest dto, IBarberShopService barberShopService)
    {
        try
        {
            var oie = await barberShopService.UpdateAsync(id, dto);
            // var barberShop = await context.BarberShops.SingleOrDefaultAsync(b => b.Id == id);

            // if (barberShop is null)
            //     return Results.NotFound();

            // barberShop.Name = dto.Name;
            // barberShop.Description = dto.Description;
            // barberShop.PhoneNumber = dto.PhoneNumber;
            // barberShop.ComercialNumber = dto.ComercialNumber;
            // barberShop.ComercialEmail = dto.ComercialEmail;
            // barberShop.OpeningHours = dto.OpeningHours;
            // barberShop.ClosingHours = dto.ClosingHours;
            // barberShop.DaysOpen = dto.DaysOpen;

            // if (dto.Address is not null)
            // {
            //     barberShop.Address.Street = dto.Address.Street;
            //     barberShop.Address.Number = dto.Address.Number;
            //     barberShop.Address.Complement = dto.Address.Complement;
            //     barberShop.Address.Neighborhood = dto.Address.Neighborhood;
            //     barberShop.Address.City = dto.Address.City;
            //     barberShop.Address.State = dto.Address.State;
            //     barberShop.Address.PostalCode = dto.Address.PostalCode;
            //     barberShop.Address.Country = dto.Address.Country;
            // }

            // barberShop.UpdatedAt = DateTime.UtcNow;

            // await context.SaveChangesAsync();
            // return Results.Ok(new { Message = "Barbearia atualizada com sucesso" });
            return Results.Ok(oie);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> DeleteBarberShop(int id, AppDbContext context)
    {
        try
        {
            var barberShop = await context.BarberShops.SingleOrDefaultAsync(b => b.Id == id);

            if (barberShop is null)
                return Results.NotFound();

            barberShop.UpdatedAt = DateTime.UtcNow;
            barberShop.IsDeleted = true;

            await context.SaveChangesAsync();
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
