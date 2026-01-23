using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using Testcontainers.MongoDb;

namespace SearchService.IntegrationTests.Fixtures
{
    // Simple WebApplicationFactory that runs a MongoDB testcontainer and exposes its connection string
    public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder().Build();

        public string MongoConnectionString { get; private set; } = string.Empty;

        public async Task InitializeAsync()
        {
            await _mongoContainer.StartAsync();
            MongoConnectionString = _mongoContainer.GetConnectionString();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Inject the test container connection string into app configuration so the app uses it
            builder.ConfigureAppConfiguration((ctx, conf) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:MongoDbConnection"] = MongoConnectionString
                };
                conf.AddInMemoryCollection(settings);
            });

            // Ensure the MongoDB.Entities DB is initialized for the test process before requests are handled.
            // This prevents the app's controllers from hitting uninitialized DB state and throwing 500.
            builder.ConfigureTestServices(services =>
            {
                // Initialize DB and create text indexes same as the real app would do on startup.
                // Use the same database name the app expects ("SearchDb").
                var mongoSettings = MongoClientSettings.FromConnectionString(MongoConnectionString);
                DB.InitAsync("SearchDb", mongoSettings).GetAwaiter().GetResult();

                // Create text indexes that the application code expects (Make/Model/Color)

                // With this (use the static Instance method to get a DB object):
                DB.Index<Item>()
                    .Key(i => i.Make, KeyType.Text)
                    .Key(i => i.Model, KeyType.Text)
                    .Key(i => i.Color, KeyType.Text)
                    .CreateAsync().GetAwaiter().GetResult();

                // No additional DI changes required here for basic integration tests.
                // Tests can further override services if needed.
            });
        }

        public async Task DisposeAsync()
        {
            await _mongoContainer.StopAsync();
            await _mongoContainer.DisposeAsync();
        }
    }
}