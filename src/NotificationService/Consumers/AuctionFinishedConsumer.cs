using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers
{
    public class AuctionFinishedConsumer(IHubContext<NotificationHub> _hubContext) : IConsumer<AuctionFinished>
    {
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("AuctionFinished Message received.");
            await _hubContext.Clients.All.SendAsync("AuctionFinished", context.Message);
        }
    }
}
