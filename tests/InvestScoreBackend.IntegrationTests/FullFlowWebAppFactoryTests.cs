using System.Net.Http.Json;
using InvestScoreBackend.Domain.Models;
using InvestScoreBackend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;

namespace InvestScoreBackend.IntegrationTests
{
    public class FullFlowWebAppFactoryTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public FullFlowWebAppFactoryTests(WebApplicationFactory<Program> factory)
        {
            // Configura a factory para usar banco em memória
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove contexto real
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<InvestScoreDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Adiciona contexto em memória
                    services.AddDbContext<InvestScoreDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("WebAppFactoryDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets()
        {
            // 1. Criar FileRecord (POST /api/filerecords)
            var fileRecordDto = new FileRecordCreateDto
            {
                FilePath = "ativos.txt",
                Title = "Ativos B3",
                Description = "Teste fluxo completo"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/filerecords", fileRecordDto);
            createResponse.EnsureSuccessStatusCode();

            var fileRecord = await createResponse.Content.ReadFromJsonAsync<FileRecordResponseDto>();
            fileRecord.Should().NotBeNull();

            // 2. Executar Prompt (POST /api/prompts/{id}/execute)
            var executeResponse = await _client.PostAsync($"/api/prompts/{fileRecord.Id}/execute", null);
            executeResponse.EnsureSuccessStatusCode();

            var executeJson = await executeResponse.Content.ReadFromJsonAsync<dynamic>();
            ((IEnumerable<object>)executeJson.assets).Should().NotBeEmpty();

            // 3. Consultar ativos (GET /api/assets)
            var assetsResponse = await _client.GetAsync("/api/assets");
            assetsResponse.EnsureSuccessStatusCode();

            var assets = await assetsResponse.Content.ReadFromJsonAsync<IEnumerable<AssetResponseDto>>();
            assets.Should().NotBeEmpty();
        }
    }
}
