using System.Globalization;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(IAuctionRepository auctionRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _auctionRepository = auctionRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAuctions([FromQuery] string date)
        {
           return await _auctionRepository.GetAuctionsAsync(date);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
        {
            var auctionDTO = await _auctionRepository.GetAuctionByIdAsync(id);
            if (auctionDTO == null)
            {
                return NotFound();
            }
            return Ok(auctionDTO);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction([FromBody] CreateAuctionDTO auctionDTO)
        {
            if (auctionDTO == null)
            {
                return BadRequest();
            }
            var auction = _mapper.Map<Auction>(auctionDTO);

            auction.Seller = User?.Identity?.Name ?? "anonymous";

            _auctionRepository.AddAuction(auction);

            var newAuction = _mapper.Map<AuctionDTO>(auction);

            // with MassTransint Entity Framework, publishing the message and saving the changes
            // in this current Postgres db are part of the same transaction
            // so if one failes, both will fail -> no databases inconsistency!
            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            var result = await _auctionRepository.SaveChangesAsync();

            if(!result) return BadRequest("Could not create auction");
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, newAuction);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, [FromBody] UpdateAuctionDTO auctionDTO)
        {
            var auction = await _auctionRepository.GetAuctionEntityByIdAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            if (User?.Identity?.Name != auction.Seller)
            {
                return Forbid();
            }
            auction.Item.Make = auctionDTO.Make ?? auction.Item.Make;
            auction.Item.Model = auctionDTO.Model ?? auction.Item.Model;
            auction.Item.Color = auctionDTO.Color ?? auction.Item.Color;
            auction.Item.Year = auctionDTO.Year ?? auction.Item.Year;
            auction.Item.Mileage = auctionDTO.Mileage ?? auction.Item.Mileage;
            auction.UpdatedAt = DateTime.UtcNow;

            var auctionToPublish = _mapper.Map<AuctionUpdated>(auctionDTO);
            auctionToPublish.Id = id.ToString();
            await _publishEndpoint.Publish(auctionToPublish);

            var result = await _auctionRepository.SaveChangesAsync();
            if (result) return Ok();
            return BadRequest("Problem saving changes");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _auctionRepository.GetAuctionEntityByIdAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            if (auction.Seller != User.Identity.Name)
            {
                return Forbid();
            }
            _auctionRepository.RemoveAuction(auction);

            await _publishEndpoint.Publish(new AuctionDeleted { Id = auction.Id.ToString() });

            var result  = await _auctionRepository.SaveChangesAsync() ;

            if (result) return NoContent();
            return BadRequest("Problem removing Auction");
        }
    }
}
