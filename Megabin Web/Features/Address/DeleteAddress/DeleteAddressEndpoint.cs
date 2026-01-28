using MediatR;

namespace Megabin_Web.Features.Address.DeleteAddress
{
    public static class DeleteAddressEndpoint
    {
        public static void MapDeleteAddressEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete(
                "DeleteAddress/{id}",
                async (Guid id, ISender sender) =>
                {
                    return await sender.Send(new DeleteAddressCommand(id));
                }
            );
        }
    }
}
