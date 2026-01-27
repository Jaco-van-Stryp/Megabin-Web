using MediatR;

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
                async (UpdateScheduleContractCommand command, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(command);
                        return Results.Ok();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        return Results.NotFound(ex.Message);
                    }
                }
            );
            return app;
        }
    }
}
