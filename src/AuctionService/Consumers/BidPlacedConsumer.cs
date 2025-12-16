using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer(AuctionDbContext _dbContext) : IConsumer<BidPlaced>
    {
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consuming bid placed");
            var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);
           if (context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBid)
           {
                auction.CurrentHighBid = context.Message.Amount;
                await _dbContext.SaveChangesAsync();
           }
        }
    }
}
