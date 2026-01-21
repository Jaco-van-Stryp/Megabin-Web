using MediatR;
using Megabin_Web.Shared.DTOs.Routing;
using Megabin_Web.Shared.Infrastructure.MapBoxService;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Megabin_Web.Features.Address.AutoComplete
{
    public class AutoCompleteHandler : IRequestHandler<AutoCompleteQuery, List<AddressSuggestion>>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapboxService _mapboxService;

        public AutoCompleteHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _mapboxService = _serviceProvider.GetRequiredService<IMapboxService>();
        }

        public async Task<List<AddressSuggestion>> Handle(
            AutoCompleteQuery request,
            CancellationToken cancellationToken
        )
        {
            string decodedAddress = Uri.UnescapeDataString(request.Address);
            var results = await _mapboxService.AutocompleteAsync(decodedAddress);
            return results;
        }
    }
}
