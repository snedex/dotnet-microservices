
using System.Diagnostics;
using Play.Inventory.Services.Data;

namespace Play.Inventory.Services.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient client;

        public CatalogClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IReadOnlyCollection<CatalogItemDTO>> GetCatalogItemsAsync()
        {
            var items = await client.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDTO>>("items");
            return items;
        }
    }
}