using MediatR;

namespace Megabin_Web.Features.Admin.ResetUserPassword;

public record ResetUserPasswordCommand(Guid UserId, string NewPassword) : IRequest;
