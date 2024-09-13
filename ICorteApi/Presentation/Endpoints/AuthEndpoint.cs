﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace ICorteApi.Presentation.Endpoints;

public static class AuthEndpoint
{
    private static readonly string INDEX = "";
    private static readonly string ENDPOINT_PREFIX = EndpointPrefixes.Auth;
    private static readonly string ENDPOINT_NAME = EndpointNames.Auth;

    public static IEndpointRouteBuilder MapAuthEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ENDPOINT_PREFIX)
            .WithTags(ENDPOINT_NAME);

        group.MapIdentityApi<User>();

        group.MapPost("logout", LogoutUser);

        return app;
    }

    public static async Task<IResult> LogoutUser(SignInManager<User> signInManager, [FromBody] object? empty)
    {
        if (empty is null)
            return Results.Unauthorized();

        await signInManager.SignOutAsync();
        return Results.StatusCode(StatusCodes.Status205ResetContent);
    }
}
