using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Utils;
using Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests
{
    [Collection("Shared collection")]
    public class AuctionBusTests : IAsyncLifetime
    {
        private readonly CustomWebAppFactory _factory;
        private readonly HttpClient _httpClient;
        private readonly ITestHarness _testHarness;
        public AuctionBusTests(CustomWebAppFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
            _testHarness = factory.Services.GetTestHarness();
        }
        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
            DbHelper.ReinitDbForTests(db);
            return Task.CompletedTask;
        }
        private static CreateAuctionDTO GetAuctionForCreate()
        {
            return new CreateAuctionDTO
            {
                Make = "test",
                Model = "testModel",
                ImageUrl = "testUrl",
                Color = "testColor",
                Mileage = 10,
                Year = 10,
                ReservePrice = 10
            };
        }

        [Fact]
        public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
        {
            // Arrange
            var auction = GetAuctionForCreate();
            _httpClient.SetFakeBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(await _testHarness.Published.Any<AuctionCreated>());
        }
    }
}
