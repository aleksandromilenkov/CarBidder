using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            Console.WriteLine("--> Consuming faulty creation of auction");

            Console.WriteLine("Do something with the failed message ( change-fix something with the message and publish it again with await context.Publish(context.Message.Message)"); 
           
        }
    }
}
