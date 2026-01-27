using MediatR;
using Megabin_Web.Shared.DTOs.Auth;

namespace Megabin_Web.Features.Auth.Register;

public record RegisterCommand(string Name, string Email, string Password, string PhoneNumber)
    : IRequest<LoginResponse>;
