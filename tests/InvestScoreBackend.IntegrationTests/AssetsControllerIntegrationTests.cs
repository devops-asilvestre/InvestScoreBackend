using FluentAssertions;
using InvestScoreBackend.API.Controllers;
using InvestScoreBackend.Application.Services;
using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Interfaces.Services;
using InvestScoreBackend.Domain.Models;
using InvestScoreBackend.Infrastructure.Persistence;
using InvestScoreBackend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InvestScoreBackend.IntegrationTests.Controllers
{
    public class AssetsControllerIntegrationTests
    {
        private readonly InvestScoreDbContext _dbContext;
        private readonly IAssetRepository _assetRepo;
        private readonly IAssetService _assetService;
        private readonly IPromptService _promptService;
        private readonly AssetsController _controller;

        public AssetsControllerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<InvestScoreDbContext>()
                .UseInMemoryDatabase("AssetsIntegrationDb")
                .Options;

            _dbContext = new InvestScoreDbContext(options);
            _assetRepo = new AssetRepository(_dbContext);
            _assetService = new AssetService(_assetRepo);

            // PromptService não será usado de verdade aqui, podemos mockar ou passar null
            _promptService = new MockPromptService();

            _controller = new AssetsController(_assetService, _promptService);
        }

        // GET: api/assets
        [Fact]
        public async Task GetAssets_Should_Return_All_Assets()
        {
            _dbContext.Assets.Add(new Asset { Ticker = "AAPL", ROE = 20, DY = 1.5, CAGR = 10, Liquidez = 1000, Risco = "Baixo", Score = 15 });
            _dbContext.SaveChanges();

            var result = await _controller.GetAssets();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as IEnumerable<AssetResponseDto>;
            response.Should().ContainSingle(a => a.Ticker == "AAPL");
        }

        // GET: api/assets/{id}
        [Fact]
        public async Task GetAsset_Should_Return_Asset_When_Found()
        {
            var asset = new Asset { Ticker = "MSFT", ROE = 30, DY = 1.2, CAGR = 12, Liquidez = 2000, Risco = "Baixo", Score = 18 };
            _dbContext.Assets.Add(asset);
            _dbContext.SaveChanges();

            var result = await _controller.GetAsset(asset.Id);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as AssetResponseDto;
            response.Ticker.Should().Be("MSFT");
        }

        // POST: api/assets
        [Fact]
        public async Task CreateAsset_Should_Persist_Asset()
        {
            var dto = new AssetCreateDto
            {
                Ticker = "TSLA",
                ROE = 15,
                DY = 0,
                CAGR = 25,
                Liquidez = 3000,
                Risco = "Médio",
                Score = 10
            };

            var result = await _controller.CreateAsset(dto);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            var response = createdResult.Value as AssetResponseDto;
            response.Ticker.Should().Be("TSLA");

            _dbContext.Assets.Any(a => a.Ticker == "TSLA").Should().BeTrue();
        }

        // PUT: api/assets/{id}
        [Fact]
        public async Task UpdateAsset_Should_Modify_Existing_Asset()
        {
            var asset = new Asset { Ticker = "PETR4", ROE = 10, DY = 5, CAGR = 8, Liquidez = 4000, Risco = "Alto", Score = 7 };
            _dbContext.Assets.Add(asset);
            _dbContext.SaveChanges();

            var dto = new AssetCreateDto { Ticker = "PETR4", ROE = 20, DY = 6, CAGR = 9, Liquidez = 5000, Risco = "Médio", Score = 12 };

            var result = await _controller.UpdateAsset(asset.Id, dto);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as AssetResponseDto;
            response.ROE.Should().Be(20);

            var updated = _dbContext.Assets.Find(asset.Id);
            updated.ROE.Should().Be(20);
        }

        // DELETE: api/assets/{id}
        [Fact]
        public async Task DeleteAsset_Should_Remove_Asset()
        {
            var asset = new Asset { Ticker = "VALE3", ROE = 25, DY = 4, CAGR = 11, Liquidez = 6000, Risco = "Baixo", Score = 20 };
            _dbContext.Assets.Add(asset);
            _dbContext.SaveChanges();

            var result = await _controller.DeleteAsset(asset.Id);

            result.Should().BeOfType<NoContentResult>();
            _dbContext.Assets.Find(asset.Id).Should().BeNull();
        }
    }

    /// <summary>
    /// Mock simples para IPromptService, usado apenas para não quebrar construtor.
    /// </summary>
    public class MockPromptService : IPromptService
    {
        public Task<List<Asset>> ExecutePromptAsync(int fileRecordId) =>
            Task.FromResult(new List<Asset>());

        public Task<Asset> FetchAssetFromAlphaVantageAsync(string ticker) =>
            Task.FromResult(new Asset { Ticker = ticker, ROE = 10, DY = 1, CAGR = 5, Liquidez = 1000, Risco = "Baixo", Score = 8 });
    }
}
