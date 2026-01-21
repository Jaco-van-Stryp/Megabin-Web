using MediatR;

namespace Megabin_Web.Features.Address.DeleteAddress
{
    public record DeleteAddressCommand(Guid id) : IRequest<IResult>;
}
