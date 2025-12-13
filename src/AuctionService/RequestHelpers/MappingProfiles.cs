using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Also look at the Item property of Auction and map its properties into this DTO:
            CreateMap<Auction, AuctionDTO>().IncludeMembers(x => x.Item);

            // AutoMapper needs to know how to map an Item into AuctionDTO because of IncludeMembers up above
            CreateMap<Item, AuctionDTO>();

            // When creating an Auction from CreateAuctionDTO, create the Item property by mapping the DTO itself into the Item property.
            CreateMap<CreateAuctionDTO, Auction>().ForMember(d => d.Item, o => o.MapFrom(s => s));

            // Necessary because of the previous mapping
            CreateMap<CreateAuctionDTO, Item>();

            CreateMap<AuctionDTO, AuctionCreated>();
        }
    }
}
