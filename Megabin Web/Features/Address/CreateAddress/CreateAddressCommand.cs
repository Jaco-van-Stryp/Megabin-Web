using MediatR;
using Megabin_Web.Shared.Domain.Enums;
using Megabin_Web.Shared.DTOs.Routing;

namespace Megabin_Web.Features.Address.CreateAddress;

public record CreateAddressCommand(
    Guid UserId,
    AddressSuggestion Address,
    int TotalBins,
    string AddressNotes,
    AddressStatus Status
) : IRequest<CreateAddressResponseDto>;
