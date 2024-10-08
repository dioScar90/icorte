﻿using Microsoft.AspNetCore.Mvc;

namespace ICorteApi.Presentation.Endpoints;

public static class ChatEndpoint
{
    private static readonly string INDEX = "";
    private static readonly string ENDPOINT_PREFIX = EndpointPrefixes.Appointment + "/{appointmentId}/" + EndpointPrefixes.Chat;
    private static readonly string ENDPOINT_NAME = EndpointNames.Chat;

    public static IEndpointRouteBuilder MapMessageEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ENDPOINT_PREFIX)
            .WithTags(ENDPOINT_NAME);

        group.MapGet("check", IsAllowedCheck)
            .RequireAuthorization(nameof(PolicyUserRole.ClientOrHigh));

        group.MapPost(INDEX, CreateMessage)
            .RequireAuthorization(nameof(PolicyUserRole.ClientOrHigh));

        group.MapGet("{id}", GetMessage)
            .RequireAuthorization(nameof(PolicyUserRole.ClientOrHigh));

        group.MapGet(INDEX, GetAllMessages)
            .RequireAuthorization(nameof(PolicyUserRole.ClientOrHigh));

        group.MapDelete("{id}", DeleteMessage)
            .RequireAuthorization(nameof(PolicyUserRole.ClientOrHigh));
            
        return app;
    }

    public static IResult GetCreatedResult(int newId, int appointmentId)
    {
        string uri = EndpointPrefixes.Appointment + "/" + appointmentId + "/" + EndpointPrefixes.Chat + "/" + newId;
        object value = new { Message = "Mensagem enviada com sucesso" };
        return Results.Created(uri, value);
    }

    public static async Task<IResult> IsAllowedCheck(
        int appointmentId,
        IMessageService service,
        IUserService userService)
    {
        int userId = await userService.GetMyUserIdAsync();
        var result = await service.CanSendMessageAsync(appointmentId, userId);
        return Results.Ok(result);
    }

    public static async Task<IResult> CreateMessage(
        int appointmentId,
        MessageDtoCreate dto,
        IMessageService service,
        IUserService userService)
    {
        int senderId = await userService.GetMyUserIdAsync();
        var message = await service.CreateAsync(dto, appointmentId, senderId);
        return GetCreatedResult(message.Id, message.AppointmentId);
    }

    public static async Task<IResult> GetMessage(
        int id,
        int appointmentId,
        IMessageService service)
    {
        var message = await service.GetByIdAsync(id, appointmentId);
        return Results.Ok(message);
    }

    public static async Task<IResult> GetAllMessages(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        int appointmentId,
        IMessageService service)
    {
        var messages = await service.GetAllAsync(page, pageSize, appointmentId);
        return Results.Ok(messages);
    }

    public static async Task<IResult> DeleteMessage(
        int appointmentId,
        int id,
        IMessageService service,
        IUserService userService)
    {
        int senderId = await userService.GetMyUserIdAsync();
        await service.DeleteAsync(id, appointmentId, senderId);
        return Results.NoContent();
    }
}
