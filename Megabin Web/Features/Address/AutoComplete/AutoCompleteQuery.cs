using MediatR;
using Megabin_Web.Shared.DTOs.Routing;

namespace Megabin_Web.Features.Address.AutoComplete
{
    public record AutoCompleteQuery(string Address) : IRequest<List<AddressSuggestion>>;

    // record is the modern way of defining DTOs in C# - it's a simple data container
    // Query holds the input data (The user types an address, and this is the input data)
    // IRequest means "When handled, this returns a List of AddressSuggestion DTOs"
}
