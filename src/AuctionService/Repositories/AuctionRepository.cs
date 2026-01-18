using System.Globalization;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repositories
{
    public class AuctionRepository(AuctionDbContext context, IMapper mapper) : IAuctionRepository
    {
        public void AddAuction(Auction auction)
        {
            context.Auctions.Add(auction);
        }

        public async Task<AuctionDTO?> GetAuctionByIdAsync(Guid id)
        {
            return await context.Auctions.ProjectTo<AuctionDTO>(mapper.ConfigurationProvider).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Auction?> GetAuctionEntityByIdAsync(Guid id)
        {
            return await context.Auctions.Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<AuctionDTO>> GetAuctionsAsync(string date)
        {
            var query = context.Auctions.OrderBy(a => a.Item.Make).AsQueryable();
            if (!string.IsNullOrWhiteSpace(date))
            {
                if (DateTime.TryParse(date, null, DateTimeStyles.RoundtripKind, out var parsed))
                {
                    query = query.Where(a => a.UpdatedAt > parsed);
                }
            }
            return await query.ProjectTo<AuctionDTO>(mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveAuction(Auction auction)
        {
            context.Auctions.Remove(auction);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
