using MediatR;
using Megabin_Web.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Megabin_Web.Features.Auth.Login
{
    public static class LoginEndpoint
    {
        public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost(
                "login",
                async ([FromBody] LoginRequest request, ISender sender) =>
                {
                    try
                    {
                        var command = new LoginCommand(request.Email, request.Password);
                        var result = await sender.Send(command);
                        return Results.Ok(result);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return Results.Unauthorized();
                    }
                }
            );
            return app;
        }
    }
}
