using MediatR;
using Megabin_Web.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Megabin_Web.Features.Auth.Register
{
    public static class RegisterEndpoint
    {
        public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost(
                "register",
                async ([FromBody] RegisterRequest request, ISender sender) =>
                {
                    try
                    {
                        var command = new RegisterCommand(
                            request.Name,
                            request.Email,
                            request.Password,
                            request.PhoneNumber
                        );
                        var result = await sender.Send(command);
                        return Results.Created("/api/Auth/me", result);
                    }
                    catch (InvalidOperationException ex)
                    {
                        return Results.BadRequest(
                            new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest)
                        );
                    }
                }
            );
            return app;
        }
    }
}
