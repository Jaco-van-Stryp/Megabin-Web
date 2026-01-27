using MediatR;
using Megabin_Web.Shared.Domain.Enums;
using Megabin_Web.Shared.DTOs.Routing;

namespace Megabin_Web.Features.Admin.UpdateUserAddress;

public record UpdateUserAddressCommand(
    Guid AddressId,
    int TotalBins,
    string AddressNotes,
    AddressStatus Status,
    Location Location
) : IRequest;
