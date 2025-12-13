using MassTransit;
using Contracts;
using AutoMapper;
using SearchService.Models;
using MongoDB.Entities;

namespace SearchService.Consumers
{
    public class AuctionUpdatedConsumer(IMapper _mapper) : IConsumer<AuctionUpdated>
    {
        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            Console.WriteLine("--> Consuming auction updated: " + context.Message.Id);

            var message = context.Message;

            // Use DB.Update to perform a partial update directly on the document in MongoDB.
            // This is highly efficient for updating specific fields.
            var result = await DB.Update<Item>()
                                 .MatchID(message.Id) // Target the document by its ID
                                 .Modify(b => b.Set(i => i.Make, message.Make))
                                 .Modify(b => b.Set(i => i.Model, message.Model))
                                 .Modify(b => b.Set(i => i.Year, message.Year))
                                 .Modify(b => b.Set(i => i.Color, message.Color))
                                 .Modify(b => b.Set(i => i.Mileage, message.Mileage))
                                 .Modify(b => b.Set(i => i.UpdatedAt, DateTime.UtcNow))
                                 .ExecuteAsync();

            if (result.ModifiedCount == 0)
            {
                // This is a critical check to see if the item was found and updated.
                Console.WriteLine($"Item with ID: {message.Id} not found or no changes applied.");
            }
            else if (!result.IsAcknowledged)
            {
                throw new MessageException(typeof(AuctionUpdated), "Problem updating mongodb.");
            }
            else
            {
                Console.WriteLine($"Item {message.Id} successfully updated via direct command.");
            }
        }
    }
}
