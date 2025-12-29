using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.Services
{
    public class GrpcAuctionService : GrpcAuction.GrpcAuctionBase
    {
        private readonly AuctionDbContext dbContext;

        public GrpcAuctionService(AuctionDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
        {
            Console.WriteLine("==> Received gRPC request for Auction ID: " + request.Id);
            var auction = await dbContext.Auctions.FindAsync(Guid.Parse(request.Id));
            if (auction == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Auction with ID {request.Id} not found."));
            }
            var response = new GrpcAuctionResponse
            {
                Auction = new GrpcAuctionModel
                {
                    AuctionEnd = auction.AuctionEnd.ToString(),
                    Id = auction.Id.ToString(),
                    ReservedPrice = auction.ReservePrice,
                    Seller = auction.Seller
                }
            };
            return response;
        }
    }
}
