using System.Net;
using System.Net.Http.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.IntegrationTests.Fixtures;
using SearchService.Models;

namespace SearchService.IntegrationTests
{
    public class SearchControllerIntegrationTests : IClassFixture<CustomWebAppFactory>
    {
        private readonly CustomWebAppFactory _factory;
        private readonly HttpClient _client;

        public SearchControllerIntegrationTests(CustomWebAppFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task InitMongoAsync()
        {
            // Initialize the MongoDB.Entities DB and seed some items for testing
            var settings = MongoClientSettings.FromConnectionString(_factory.MongoConnectionString);
            await DB.InitAsync("TestSearchDb", settings);

            // Remove the data initialy
            await DB.DeleteAsync<Item>(DB.Filter<Item>().Empty);

            var items = new[]
            {
                new Item
                {
                    Seller = "alice",
                    Winner = null,
                    CreatedAt = System.DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = System.DateTime.UtcNow.AddDays(-1),
                    AuctionEnd = System.DateTime.UtcNow.AddDays(5),
                    Status = "Live",
                    ReservePrice = 1000,
                    SoldAmount = 0,
                    CurrentHighBid = 0,
                    Make = "Ford",
                    Model = "Fiesta",
                    Year = 2019,
                    Color = "Blue",
                    Mileage = 30000,
                    ImageUrl = "url1"
                },
                new Item
                {
                    Seller = "bob",
                    Winner = null,
                    CreatedAt = System.DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = System.DateTime.UtcNow,
                    AuctionEnd = System.DateTime.UtcNow.AddDays(1),
                    Status = "Live",
                    ReservePrice = 2000,
                    SoldAmount = 0,
                    CurrentHighBid = 0,
                    Make = "Toyota",
                    Model = "Corolla",
                    Year = 2020,
                    Color = "White",
                    Mileage = 20000,
                    ImageUrl = "url2"
                }
            };
            await DB.SaveAsync(items);
        }

        [Fact]
        public async Task SearchItems_ReturnsAllResults()
        {
            // Arrange
            await InitMongoAsync();

            // Act
            var response = await _client.GetAsync("/api/search");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var payload = await response.Content.ReadFromJsonAsync<SearchResponse>();
            Assert.NotNull(payload);
            Assert.NotEmpty(payload.results);
        }

        [Fact]
        public async Task SearchItems_FilterBySeller_ReturnsOnlyThatSeller()
        {
            // Arrange
            await InitMongoAsync();

            // Act
            var response = await _client.GetAsync("/api/search?seller=bob");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var payload = await response.Content.ReadFromJsonAsync<SearchResponse>();
            Assert.NotNull(payload);
            Assert.All(payload.results, i => Assert.Equal("bob", i.Seller));
        }

        // Local helper types to deserialize the controller response
        private record SearchResponse(List<ItemDto> results, int pageCount, int totalCount);

        // Minimal DTO subset used by the controller response deserialization
        private class ItemDto
        {
            public string Seller { get; set; } = default!;
            public string Make { get; set; } = default!;
            public string Model { get; set; } = default!;
            public string Color { get; set; } = default!;
            public string ImageUrl { get; set; } = default!;
            public int Year { get; set; }
            public int Mileage { get; set; }
            public int ReservePrice { get; set; }
        }
    }
}