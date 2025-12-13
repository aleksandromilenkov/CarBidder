using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer(IMapper _mapper) : IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("--> Consuming auction created: "+ context.Message.Id);
            // map to Item and write it to the datbase:
            var item = _mapper.Map<Item>(context.Message);
            await item.SaveAsync();
        }
    }
}
