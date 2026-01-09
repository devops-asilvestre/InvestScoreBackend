using InvestScoreBackend.API.Controllers;
using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;

namespace InvestScoreBackend.UnitTests.Controllers
{
    public class PromptsControllerTests
    {
        private readonly Mock<IPromptService> _promptServiceMock;
        private readonly PromptsController _controller;

        public PromptsControllerTests()
        {
            _promptServiceMock = new Mock<IPromptService>();
            _controller = new PromptsController(_promptServiceMock.Object);
        }

        // Caso de sucesso
        [Fact]
        public async Task ExecutePrompt_Should_Return_Assets_When_Success()
        {
            // Arrange
            var assets = new List<Asset>
            {
                new Asset { Ticker = "AAPL", ROE = 20, DY = 1.5, CAGR = 10, Liquidez = 1000, Risco = "Baixo", Score = 15 }
            };

            _promptServiceMock.Setup(s => s.ExecutePromptAsync(1)).ReturnsAsync(assets);

            // Act
            var result = await _controller.ExecutePrompt(1);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            dynamic response = okResult.Value;
            ((IEnumerable<object>)response.assets).Should().HaveCount(1);
            ((IEnumerable<object>)response.assets).First().Should().BeEquivalentTo(new
            {
                Ticker = "AAPL",
                ROE = 20.0,
                DY = 1.5,
                CAGR = 10.0,
                Liquidez = 1000.0,
                Risco = "Baixo",
                Score = 15.0
            });
        }

        // Caso de erro (OpenAI não retorna ativos)
        [Fact]
        public async Task ExecutePrompt_Should_Return_BadRequest_When_ServiceThrowsException()
        {
            // Arrange
            _promptServiceMock.Setup(s => s.ExecutePromptAsync(1))
                .ThrowsAsync(new Exception("OpenAI não retornou ativos válidos."));

            // Act
            var result = await _controller.ExecutePrompt(1);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            ((dynamic)badRequestResult.Value).message.Should().Be("OpenAI não retornou ativos válidos.");
        }
    }
}
