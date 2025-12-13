using System.Globalization;
using AuctionService.Data;
using AuctionService.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
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
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAuctions([FromQuery] string? date)
        {
            var query = _context.Auctions.OrderBy(a => a.Item.Make).AsQueryable();
            if (!string.IsNullOrWhiteSpace(date))
            {
                if (DateTime.TryParse(date, null, DateTimeStyles.RoundtripKind, out var parsed))
                {
                    query = query.Where(a => a.UpdatedAt > parsed);
                }
                else
                {
                    return BadRequest("Invalid date format");
                }
            }
            return await query.ProjectTo<AuctionDTO>(_mapper.ConfigurationProvider).ToListAsync();
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

            var newAuction = _mapper.Map<AuctionDTO>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            var result = await _context.SaveChangesAsync() > 0;

            if(!result) return BadRequest("Could not create auction");
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, newAuction);
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
