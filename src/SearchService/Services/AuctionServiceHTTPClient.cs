using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services
{
    public class AuctionServiceHTTPClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AuctionServiceHTTPClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<Item>> GetItemsForSearchDb()
        {
            var lastUpdated = await DB.Find<Item, DateTime>()
                .Sort(i => i.Descending(i => i.UpdatedAt))
                .Project(i => i.UpdatedAt)
                .ExecuteFirstAsync();
            //:O = ISO 8601 round-trip format, ALWAYS parses correctly.
            var items = await _httpClient.GetFromJsonAsync<List<Item>>(_config["AuctionServiceUrl"] + $"/api/auctions?date={lastUpdated:O}");
            return items ?? new List<Item>();
        }
    }
}
