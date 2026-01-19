using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using AuctionService.Data;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using AuctionService.IntegrationTests.Utils;

namespace AuctionService.IntegrationTests.Fixtures
{
    public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<AuctionDbContext>();

                services.AddDbContext<AuctionDbContext>(options =>
                {
                    options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
                });

                services.AddMassTransitTestHarness();
                services.EnsureCreated<AuctionDbContext>();
            });
        }

        Task IAsyncLifetime.DisposeAsync() => _postgreSqlContainer.DisposeAsync().AsTask();


    }
}
