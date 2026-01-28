using MediatR;

namespace Megabin_Web.Features.Address.CreateAddress
{
    public static class CreateAddressEndpoint
    {
        public static IEndpointRouteBuilder MapCreateAddressEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost(
                "CreateAddress",
                async (CreateAddressCommand command, ISender sender) =>
                {
                    return await sender.Send(command);
                }
            );
            return app;
        }
    }
}
