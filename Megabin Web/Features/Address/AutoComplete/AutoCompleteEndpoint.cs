using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Megabin_Web.Features.Address.AutoComplete
{
    public static class AutoCompleteEndpoint
    {
        public static void MapAutoCompleteEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet(
                "autocomplete/{address}",
                async (string address, ISender sender) =>
                {
                    var result = await sender.Send(new AutoCompleteQuery(address));
                    return TypedResults.Ok(result);
                }
            );
        }
    }
}
