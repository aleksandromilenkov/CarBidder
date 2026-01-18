using System;
using System.Collections.Generic;
using System.Text;
using AuctionService.Controllers;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Repositories;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
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
            _controller = new AuctionsController(_auctionRepository.Object, _mapper, _publishEndpoint.Object);
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
    }
}
