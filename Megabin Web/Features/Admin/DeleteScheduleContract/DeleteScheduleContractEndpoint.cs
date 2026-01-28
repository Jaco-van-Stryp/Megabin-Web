using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

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
                async Task<Results<Ok, NotFound<string>>> (Guid contractId, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(new DeleteScheduleContractCommand(contractId));
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
