using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionDeletedConsumer(IMapper _mapper) : IConsumer<AuctionDeleted>
    {
        public async Task Consume(ConsumeContext<AuctionDeleted> context)
        {
            var itemId = context.Message.Id;
            Console.WriteLine("--> Consuming auction deleted for ID: " + itemId);

            // 1. Await the Task<long> return value
            // 'result' will be of type 'long'
            var result = await DB.DeleteAsync<Item>(itemId);

            if (result.DeletedCount > 0) // Check the long value
            {
                Console.WriteLine($"Item {itemId} successfully deleted from MongoDB.");
            }
            else if (!result.IsAcknowledged)
            {
                throw new MessageException(typeof(AuctionUpdated), "Problem updating mongodb.");
            }
            else
            {
                // This means the item didn't exist in the database (result == 0)
                Console.WriteLine($"Item {itemId} not found in MongoDB or was not deleted. (Modified Count: {result})");
            }
        }
    }
}
