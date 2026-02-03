using MediatR;

namespace Megabin_Web.Features.Customer.RequestBin;

public record RequestBinCommand(Guid AddressId) : IRequest;
