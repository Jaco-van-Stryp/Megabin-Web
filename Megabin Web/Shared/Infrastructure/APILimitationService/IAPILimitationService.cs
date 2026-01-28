using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Shared.Infrastructure.APILimitationService
{
    public interface IAPILimitationService
    {
        Task<bool> RecordApiCallAsync(Guid? userId, APITypes apiType);
    }
}
