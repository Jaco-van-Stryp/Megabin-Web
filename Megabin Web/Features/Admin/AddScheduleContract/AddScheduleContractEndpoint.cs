using MediatR;

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
                async (AddScheduleContractCommand command, ISender sender) =>
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
