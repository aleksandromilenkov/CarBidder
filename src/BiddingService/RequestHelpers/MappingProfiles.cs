using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using Contracts;

namespace BiddingService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AuctionCreated, Auction>();
            CreateMap<Bid, BidDTO>();
            CreateMap<Bid, BidPlaced>();
        }
    }
}
