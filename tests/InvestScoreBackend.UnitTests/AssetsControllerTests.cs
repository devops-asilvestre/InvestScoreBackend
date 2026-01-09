using FluentAssertions;
using InvestScoreBackend.API.Controllers;
using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Interfaces.Services;
using InvestScoreBackend.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvestScoreBackend.UnitTests.Controllers
{
    public class AssetsControllerTests
    {
        private readonly Mock<IAssetService> _assetServiceMock;
        private readonly Mock<IPromptService> _promptServiceMock;
        private readonly AssetsController _controller;

        public AssetsControllerTests()
        {
            _assetServiceMock = new Mock<IAssetService>();
            _promptServiceMock = new Mock<IPromptService>();
            _controller = new AssetsController(_assetServiceMock.Object, _promptServiceMock.Object);
        }

        //  GET: api/assets
        [Fact]
        public async Task GetAssets_Should_Return_List_Of_Assets()
        {
            var assets = new List<Asset>
            {
                new Asset { Id = 1, Ticker = "AAPL", ROE = 20, DY = 1.5, CAGR = 10, Liquidez = 1000, Risco = "Baixo", Score = 15 }
            };
            _assetServiceMock.Setup(s => s.GetAllAssetsAsync()).ReturnsAsync(assets);

            var result = await _controller.GetAssets();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as IEnumerable<AssetResponseDto>;
            response.Should().HaveCount(1);
            response.First().Ticker.Should().Be("AAPL");
        }

        //  GET: api/assets/{id}
        [Fact]
        public async Task GetAsset_Should_Return_Asset_When_Found()
        {
            var asset = new Asset { Id = 1, Ticker = "AAPL" };
            _assetServiceMock.Setup(s => s.GetAssetAsync(1)).ReturnsAsync(asset);

            var result = await _controller.GetAsset(1);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as AssetResponseDto;
            response.Ticker.Should().Be("AAPL");
        }

        [Fact]
        public async Task GetAsset_Should_Return_NotFound_When_NotFound()
        {
            _assetServiceMock.Setup(s => s.GetAssetAsync(1)).ReturnsAsync((Asset)null);

            var result = await _controller.GetAsset(1);

            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            ((dynamic)notFoundResult.Value).message.Should().Be("Ativo não encontrado.");
        }

        // POST: api/assets
        [Fact]
        public async Task CreateAsset_Should_Return_Created_Asset()
        {
            var asset = new Asset { Id = 1, Ticker = "AAPL" };
            _promptServiceMock.Setup(p => p.FetchAssetFromAlphaVantageAsync("AAPL")).ReturnsAsync(asset);
            _assetServiceMock.Setup(s => s.CreateAssetAsync(asset)).ReturnsAsync(asset);

            var dto = new AssetCreateDto { Ticker = "AAPL" };

            var result = await _controller.CreateAsset(dto);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            var response = createdResult.Value as AssetResponseDto;
            response.Ticker.Should().Be("AAPL");
        }

        // PUT: api/assets/{id}
        [Fact]
        public async Task UpdateAsset_Should_Return_Updated_Asset()
        {
            var existing = new Asset { Id = 1, Ticker = "AAPL", ROE = 10 };
            _assetServiceMock.Setup(s => s.GetAssetAsync(1)).ReturnsAsync(existing);
            _assetServiceMock.Setup(s => s.UpdateAssetAsync(It.IsAny<Asset>())).ReturnsAsync(existing);

            var dto = new AssetCreateDto { Ticker = "AAPL", ROE = 20 };

            var result = await _controller.UpdateAsset(1, dto);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as AssetResponseDto;
            response.ROE.Should().Be(20);
        }

        [Fact]
        public async Task UpdateAsset_Should_Return_NotFound_When_Asset_Does_Not_Exist()
        {
            _assetServiceMock.Setup(s => s.GetAssetAsync(1)).ReturnsAsync((Asset)null);

            var dto = new AssetCreateDto { Ticker = "AAPL" };
            var result = await _controller.UpdateAsset(1, dto);

            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            ((dynamic)notFoundResult.Value).message.Should().Be("Ativo não encontrado.");
        }

        // DELETE: api/assets/{id}
        [Fact]
        public async Task DeleteAsset_Should_Return_NoContent_When_Success()
        {
            _assetServiceMock.Setup(s => s.DeleteAssetAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsset(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteAsset_Should_Return_NotFound_When_Asset_Does_Not_Exist()
        {
            _assetServiceMock.Setup(s => s.DeleteAssetAsync(1)).Throws(new KeyNotFoundException("Ativo com Id 1 não encontrado."));

            var result = await _controller.DeleteAsset(1);

            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            ((dynamic)notFoundResult.Value).message.Should().Be("Ativo com Id 1 não encontrado.");
        }
    }
}
