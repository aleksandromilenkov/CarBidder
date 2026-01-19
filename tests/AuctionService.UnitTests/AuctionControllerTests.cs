using System;
using System.Collections.Generic;
using System.Text;
using AuctionService.Controllers;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Repositories;
using AuctionService.RequestHelpers;
using AuctionService.UnitTests.Utils;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests
{
    public class AuctionControllerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepository;
        private readonly Mock<IPublishEndpoint> _publishEndpoint;
        private readonly Fixture _fixture;
        private readonly AuctionsController _controller;
        private readonly IMapper _mapper;
        public AuctionControllerTests()
        {
            _fixture = new Fixture();
            _auctionRepository = new Mock<IAuctionRepository>();
            _publishEndpoint = new Mock<IPublishEndpoint>();

            var mockMapper = new MapperConfiguration(mc =>
            {
                mc.AddMaps(typeof(MappingProfiles).Assembly);
            }).CreateMapper().ConfigurationProvider;

            _mapper = new Mapper(mockMapper);
            _controller = new AuctionsController(_auctionRepository.Object, _mapper, _publishEndpoint.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = Helpers.GetClaimsPrincipal() }
                }
            };
        }

        [Fact]
        public async Task GetAuctions_WithNoParams_Returns10Auctions()
        {
            // Arrange
            var auctions = _fixture.CreateMany<AuctionDTO>(10).ToList();
            _auctionRepository.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);

            // Act
            var result = await _controller.GetAuctions(null);

            // Assert
            Assert.NotNull(result.Value);
            Assert.Equal(10, result.Value.Count);
            Assert.IsType<ActionResult<List<AuctionDTO>>>(result);
        }


        [Fact]
        public async Task GetAuctionById_WithValidGuid_ReturnsAuction()
        {
            // Arrange
            var auction = _fixture.Create<AuctionDTO>();
            _auctionRepository.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

            // Act
            var result = await _controller.GetAuctionById(auction.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var auctionDto = Assert.IsType<AuctionDTO>(okResult.Value);
            Assert.Equal(auction.Make, auctionDto.Make);
        }

        [Fact]
        public async Task GetAuctionById_WithInValidGuid_ReturnsAuction()
        {
            // Arrange
            _auctionRepository.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

            // Act
            var result = await _controller.GetAuctionById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }


        [Fact]
        public async Task CreateAuction_WithValidCreateAuctionDTO_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var auction = _fixture.Create<CreateAuctionDTO>();
            _auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateAuction(auction);
            var createdResult = result.Result as CreatedAtActionResult;

            // Assert
            Assert.NotNull(createdResult);
            Assert.Equal("GetAuctionById", createdResult.ActionName);
            Assert.IsType<CreatedAtActionResult>(createdResult);
            Assert.IsType<AuctionDTO>(createdResult.Value);
        }

        [Fact]
        public async Task CreateAuction_FailedSave_Returns400BadRequest()
        {
            // Arrange
            var auction = _fixture.Create<CreateAuctionDTO>();
            _auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateAuction(auction);
            var createdResult = result.Result;

            // Assert
            Assert.NotNull(createdResult);
            Assert.IsType<BadRequestObjectResult>(createdResult);
        }

        [Fact]
        public async Task UpdateAuction_WithUpdateAuctionDto_ReturnsOkResponse()
        {
            // Arrange
            var auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            auction.Item = _fixture.Build<Item>().Without(i => i.Auction).Create();
            auction.Seller = "test";
            var auctionUpdateDTO = _fixture.Create<UpdateAuctionDTO>();
            _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAuction(auction.Id, auctionUpdateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateAuction_WithInvalidUser_Returns403Forbid()
        {
            var auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            auction.Item = _fixture.Build<Item>().Without(i => i.Auction).Create();
            auction.Seller = "anotherUser";
            var auctionUpdateDTO = _fixture.Create<UpdateAuctionDTO>();
            _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAuction(auction.Id, auctionUpdateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateAuction_WithInvalidGuid_ReturnsNotFound()
        {
            var auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            auction.Item = _fixture.Build<Item>().Without(i => i.Auction).Create();
            auction.Seller = "test";
            var auctionUpdateDTO = _fixture.Create<UpdateAuctionDTO>();
            _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value:null);
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAuction(new Guid(), auctionUpdateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAuction_WithValidUser_ReturnsOkResponse()
        {
            var auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            auction.Item = _fixture.Build<Item>().Without(i => i.Auction).Create();
            auction.Seller = "test";
            _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAuction(auction.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAuction_WithInvalidGuid_Returns404Response()
        {
            var auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            auction.Item = _fixture.Build<Item>().Without(i => i.Auction).Create();
            auction.Seller = "test";
            _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAuction(new Guid());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAuction_WithInvalidUser_Returns403Response()
        {
            var auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            auction.Item = _fixture.Build<Item>().Without(i => i.Auction).Create();
            auction.Seller = "asd";
            _auctionRepository.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAuction(auction.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ForbidResult>(result);
        }
    }
}
