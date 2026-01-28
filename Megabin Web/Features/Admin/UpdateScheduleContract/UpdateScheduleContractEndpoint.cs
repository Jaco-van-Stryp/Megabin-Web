using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.UpdateScheduleContract
{
    public static class UpdateScheduleContractEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateScheduleContractEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "UpdateScheduleContract",
                async Task<Results<Ok, NotFound<string>>> (UpdateScheduleContractCommand command, ISender sender) =>
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
