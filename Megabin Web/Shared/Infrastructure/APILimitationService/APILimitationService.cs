using Megabin_Web.Entities;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Shared.Infrastructure.APILimitationService
{
    public class APILimitationService : IAPILimitationService
    {
        private readonly AppDbContext _dbContext;
        private readonly APILimitOptions _apiLimitOptions;

        public APILimitationService(
            AppDbContext dbContext,
            IOptions<APILimitOptions> apiLimitOptions
        )
        {
            _dbContext = dbContext;
            _apiLimitOptions = apiLimitOptions.Value;
        }

        public async Task<bool> RecordApiCallAsync(Guid? userId, APITypes apiType)
        {
            //Get all requests for the day
            var apiLimit = await _dbContext
                .APIUsageTrackers.Where(x =>
                    x.UserId == userId
                    && x.ExternalApiName == apiType
                    && x.DateCreated.Date == DateTime.UtcNow.Date
                )
                .ToListAsync();

            if (HasExceededLimit(apiLimit.Count, apiType))
            {
                return false;
            }

            // Record the API call
            var apiUsage = new APIUsageTracker { ExternalApiName = apiType, UserId = userId };
            _dbContext.APIUsageTrackers.Add(apiUsage);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private bool HasExceededLimit(int currentCount, APITypes apiType)
        {
            switch (apiType)
            {
                case APITypes.OpenRouteService_OptimizeRoute:
                    return currentCount >= _apiLimitOptions.RouteGenerationPerDayLimit;
                case APITypes.Mapbox_Autocomplete:
                    return currentCount >= _apiLimitOptions.AutoCompletePerDayLimit;
                default:
                    return false;
            }
        }
    }
}
