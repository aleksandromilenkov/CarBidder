using BiddingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidsController : ControllerBase
    {
        [HttpGet("{auctionId}")]
        public async Task<ActionResult<List<Bid>>> GetBidsForAuction([FromRoute] string auctionId)
        {
            var bids = await DB.Find<Bid>()
                .Match(b => b.AuctionId == auctionId)
                .Sort(b => b.Descending(b => b.BidTime))
                .ExecuteAsync();
            return Ok(bids);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Bid>> PlaceBid([FromQuery] string auctionId, [FromQuery] int amount)
        {
            var auction = await DB.Find<Auction>().OneAsync(auctionId);

            if(auction == null)
            {
                // TODO: Check with AuctionService if that has auction
                return NotFound("Auction not found");
            }

            if (auction.Seller == User.Identity.Name)
            {
                return BadRequest("Seller cannot place bids on their own auction");
            }

            var bid = new Bid
            {
                AuctionId = auctionId,
                Bidder = User.Identity.Name ?? "anonymous",
                Amount = amount
            };

            if(auction.AuctionEnd < DateTime.UtcNow)
            {
                bid.BidStatus = BidStatus.Finished;
            } else {
                var highBid = await DB.Find<Bid>()
                   .Match(b => b.AuctionId == auctionId)
                   .Sort(b => b.Descending(b => b.Amount))
                   .ExecuteFirstAsync();

                if ((highBid != null && amount > highBid.Amount) || highBid == null)
                {
                    bid.BidStatus = amount > auction.ReservePrice ? BidStatus.Accepted : BidStatus.AcceptedBelowReserve;
                }

                if (highBid != null && amount <= highBid.Amount)
                {
                    bid.BidStatus = BidStatus.TooLow;
                }
            }
            await DB.SaveAsync(bid);
            return Ok(bid);
        } 
    }
}
