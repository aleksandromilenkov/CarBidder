using AutoMapper;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Consumers
{
    public class AuctionCreatedConsumer(IMapper _mapper) : IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("--> Consuming auction created: " + context.Message.Id);
            // map to Auction and write it to the datbase:
            var auction = _mapper.Map<Auction>(context.Message);
            await auction.SaveAsync();
        }
    }
}
