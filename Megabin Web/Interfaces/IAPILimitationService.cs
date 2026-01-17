using Megabin_Web.Enums;

namespace Megabin_Web.Interfaces
{
    public interface IAPILimitationService
    {
        Task<bool> RecordApiCallAsync(Guid? userId, APITypes apiType);
    }
}
