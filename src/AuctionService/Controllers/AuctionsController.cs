using AuctionService.Data;
using AuctionService.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAuctions()
        {
            var auctions = await _context.Auctions.Include(a => a.Item).OrderBy(a => a.Item.Make).ToListAsync();
            var auctionsDTO = _mapper.Map<List<AuctionDTO>>(auctions);
            return Ok(auctionsDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions.Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            var auctionDTO = _mapper.Map<AuctionDTO>(auction);
            return Ok(auctionDTO);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction([FromBody] CreateAuctionDTO auctionDTO)
        {
            if (auctionDTO == null)
            {
                return BadRequest();
            }
            var auction = _mapper.Map<Entities.Auction>(auctionDTO);
            // TODO: add current logged in user as seller
            auction.Seller = "test";
            _context.Auctions.Add(auction);
            var result = await _context.SaveChangesAsync() > 0;
            if(!result) return BadRequest("Could not create auction");
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, _mapper.Map<AuctionDTO>(auction));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, [FromBody] UpdateAuctionDTO auctionDTO)
        {
            var auction = await _context.Auctions.Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            // TODO: Check seller == current logged in user (its username)
            auction.Item.Make = auctionDTO.Make ?? auction.Item.Make;
            auction.Item.Model = auctionDTO.Model ?? auction.Item.Model;
            auction.Item.Color = auctionDTO.Color ?? auction.Item.Color;
            auction.Item.Year = auctionDTO.Year ?? auction.Item.Year;
            auction.Item.Mileage = auctionDTO.Mileage ?? auction.Item.Mileage;
            auction.UpdatedAt = DateTime.UtcNow;
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok();
            return BadRequest("Problem saving changes");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            _context.Auctions.Remove(auction);
            var result  = await _context.SaveChangesAsync() > 0;
            if (result) return NoContent();
            return BadRequest("Problem removing Auction");
        }
    }
}
