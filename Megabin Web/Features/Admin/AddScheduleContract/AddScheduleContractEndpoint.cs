using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.AddScheduleContract
{
    public static class AddScheduleContractEndpoint
    {
        public static IEndpointRouteBuilder MapAddScheduleContractEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "AddScheduleContract",
                async Task<Results<Ok, NotFound<string>>> (AddScheduleContractCommand command, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(command);
                        return TypedResults.Ok();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        return TypedResults.NotFound(ex.Message);
                    }
                }
            );
            return app;
        }
    }
}
