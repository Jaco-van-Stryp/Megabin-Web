using MediatR;
using Megabin_Web.Shared.DTOs.Auth;

namespace Megabin_Web.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
