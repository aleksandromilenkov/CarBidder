using AuctionService.DTOs;
using AuctionService.Entities;

namespace AuctionService.Repositories
{
    public interface IAuctionRepository
    {
        Task<List<AuctionDTO>> GetAuctionsAsync(string date);
        Task<AuctionDTO?> GetAuctionByIdAsync(Guid id);
        Task<Auction?> GetAuctionEntityByIdAsync(Guid id);
        void AddAuction(Auction auction);
        void RemoveAuction(Auction auction);
        Task<bool> SaveChangesAsync();
    }
}
