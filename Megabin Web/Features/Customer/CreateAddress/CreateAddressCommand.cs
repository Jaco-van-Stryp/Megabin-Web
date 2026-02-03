using MediatR;
using Megabin_Web.Shared.DTOs.Routing;

namespace Megabin_Web.Features.Customer.CreateAddress;

public record CustomerCreateAddressCommand(
    AddressSuggestion Address,
    int TotalBins,
    string? AddressNotes
) : IRequest<CustomerCreateAddressResponseDto>;

public record CustomerCreateAddressResponseDto
{
    public required Guid AddressId { get; init; }
}
