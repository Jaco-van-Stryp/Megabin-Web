using MediatR;
using Megabin_Web.Features.Address.CreateAddress;
using Megabin_Web.Shared.Domain.Enums;
using Megabin_Web.Shared.DTOs.Routing;

namespace Megabin_Web.Features.Admin.AddUserAddress;

public record AddUserAddressCommand(
    Guid UserId,
    AddressSuggestion Address,
    int TotalBins,
    string AddressNotes,
    AddressStatus Status
) : IRequest<CreateAddressResponseDto>;
