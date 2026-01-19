using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests
{
    [Collection("Shared collection")]
    public class AuctionControllerIntegrationTests : IAsyncLifetime
    {
        private readonly CustomWebAppFactory _factory;
        private readonly HttpClient _httpClient;
        private const string CAR_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

        public AuctionControllerIntegrationTests(CustomWebAppFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
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
        public async Task GetAuctions_ShouldReturn3Auctions()
        {
            // Arrange..

            // Act
            var response = await _httpClient.GetFromJsonAsync<List<AuctionDTO>>("api/auctions");

            // Assert
            Assert.Equal(3, response.Count());
        }

        [Fact]
        public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
        {
            // Arrange..


            // Act
            var response = await _httpClient.GetFromJsonAsync<AuctionDTO>($"api/auctions/{CAR_ID}");

            // Assert
            Assert.Equal("GT", response.Model);
        }


        [Fact]
        public async Task GetAuctionById_WithInValidId_ShouldReturn404()
        {
            // Arrange..


            // Act
            var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAuctionById_WithInValidId_ShouldReturn400()
        {
            // Arrange..


            // Act
            var response = await _httpClient.GetAsync($"api/auctions/notGuid");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_WithNoAuth_ShouldReturn401()
        {
            // Arrange
            var auction = new CreateAuctionDTO { Make = "test" };

            // Act
            var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_WithAuth_ShouldReturn201()
        {
            // Arrange
            var auction = GetAuctionForCreate();
            _httpClient.SetFakeBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDTO>();
            Assert.NotNull(createdAuction);
            Assert.Equal("bob", createdAuction.Seller);
        }

        [Fact]
        public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
        {
            // Arrange
            var auction = GetAuctionForCreate();
            auction.Make = null;
            _httpClient.SetFakeBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
        {
            // Arrange
            var updateAuctionDTO = new UpdateAuctionDTO { Make = "NewNew" };
            _httpClient.SetFakeBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            var response = await _httpClient.PutAsJsonAsync($"api/auctions/{CAR_ID}", updateAuctionDTO);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
        {
            // Arrange
            var updateAuctionDTO = new UpdateAuctionDTO { Make = "NewNew" };
            _httpClient.SetFakeBearerToken(AuthHelper.GetBearerForUser("alice"));

            // Act
            var response = await _httpClient.PutAsJsonAsync($"api/auctions/{CAR_ID}", updateAuctionDTO);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAuction_ShouldReturn204()
        {
            // Arrange
            _httpClient.SetFakeBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            var response = await _httpClient.DeleteAsync($"api/auctions/{CAR_ID}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAuction_ShouldReturn403()
        {
            // Arrange
            _httpClient.SetFakeBearerToken(AuthHelper.GetBearerForUser("alice"));

            // Act
            var response = await _httpClient.DeleteAsync($"api/auctions/{CAR_ID}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAuction_ShouldReturn404()
        {
            // Arrange
            _httpClient.SetFakeBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            var response = await _httpClient.DeleteAsync($"api/auctions/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
