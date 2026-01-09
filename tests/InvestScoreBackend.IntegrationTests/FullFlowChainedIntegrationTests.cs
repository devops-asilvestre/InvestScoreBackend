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
using InvestScoreBackend.IntegrationTests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace InvestScoreBackend.IntegrationTests.Controllers
{
    public class FullFlowChainedIntegrationTests
    {
        private readonly InvestScoreDbContext _dbContext;
        private readonly IFileRecordRepository _fileRecordRepo;
        private readonly IAssetRepository _assetRepo;
        private readonly IAssetHeadRepository _assetHeadRepo;
        private readonly IFileRecordService _fileRecordService;
        private readonly IAssetService _assetService;
        private readonly IAssetHeadService _assetHeadService;
        private readonly IPromptService _promptService;

        private readonly FileRecordsController _fileRecordsController;
        private readonly PromptsController _promptsController;
        private readonly AssetsController _assetsController;

        public FullFlowChainedIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<InvestScoreDbContext>()
                .UseInMemoryDatabase("FullFlowIntegrationDb")
                .Options;

            _dbContext = new InvestScoreDbContext(options);
            _fileRecordRepo = new FileRecordRepository(_dbContext);
            _assetRepo = new AssetRepository(_dbContext);
            _assetHeadRepo = new AssetHeadRepository(_dbContext);

            _fileRecordService = new FileRecordService(_fileRecordRepo);
            _assetService = new AssetService(_assetRepo);
            _assetHeadService = new AssetHeadService(_assetHeadRepo);

            // Usa HttpClientFactoryFake para simular OpenAI
            _promptService = new PromptService(
                _fileRecordRepo,
                _assetService,
                _assetHeadService,
                new HttpClientFactoryFake(),
                new ConfigurationBuilder().Build()
            );

            _fileRecordsController = new FileRecordsController(_fileRecordService);
            _promptsController = new PromptsController(_promptService);
            _assetsController = new AssetsController(_assetService, _promptService);
        }

        [Fact]
        public async Task FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets()
        {
            // 1. Criar FileRecord simulando ativos.txt
            var dto = new FileRecordCreateDto
            {
                FilePath = "ativos.txt",
                Title = "Ativos B3",
                Description = "Fluxo completo de integração"
            };

            var createResult = await _fileRecordsController.CreateFileRecord(dto);
            var created = createResult.Result as CreatedAtActionResult;
            created.Should().NotBeNull();

            var fileRecordResponse = created.Value as FileRecordResponseDto;
            fileRecordResponse.Should().NotBeNull();

            // 2. Executar Prompt via PromptsController
            var executeResult = await _promptsController.ExecutePrompt(fileRecordResponse.Id);
            var okExecute = executeResult as OkObjectResult;
            okExecute.Should().NotBeNull();

            dynamic executeResponse = okExecute.Value;
            ((IEnumerable<object>)executeResponse.assets).Should().HaveCount(3);

            // 3. Consultar ativos via AssetsController
            var getResult = await _assetsController.GetAssets();
            var okGet = getResult.Result as OkObjectResult;
            okGet.Should().NotBeNull();

            var assetsResponse = okGet.Value as IEnumerable<AssetResponseDto>;
            assetsResponse.Should().NotBeEmpty();
            assetsResponse.Select(a => a.Ticker).Should().Contain(new[] { "AAPL", "MSFT", "TSLA" });
        }
    }
}
