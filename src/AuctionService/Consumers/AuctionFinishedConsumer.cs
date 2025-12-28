using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionFinishedConsumer(AuctionDbContext _dbContext) : IConsumer<AuctionFinished>
    {
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("--> Consuming auction finished");
            var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));
            if(context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount;
            }

            auction.Status = auction.SoldAmount > auction.ReservePrice ? Enums.Status.Finished : Enums.Status.ReserveNotMet;
            await _dbContext.SaveChangesAsync();
        }
    }
}
