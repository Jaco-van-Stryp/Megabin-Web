using MediatR;

namespace Megabin_Web.Features.Admin.DeleteScheduleContract
{
    public static class DeleteScheduleContractEndpoint
    {
        public static IEndpointRouteBuilder MapDeleteScheduleContractEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "DeleteScheduleContract",
                async (Guid contractId, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(new DeleteScheduleContractCommand(contractId));
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
