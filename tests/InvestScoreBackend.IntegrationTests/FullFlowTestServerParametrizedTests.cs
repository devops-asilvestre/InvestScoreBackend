using System.Net.Http.Json;
using System.Text.Json;
using InvestScoreBackend.Infrastructure.Persistence;
using InvestScoreBackend.Domain.Models;
using InvestScoreBackend.IntegrationTests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;

namespace InvestScoreBackend.IntegrationTests
{
    // DTO para a resposta do /api/prompts/{id}/execute
    public class ExecutePromptResponseDto
    {
        public IEnumerable<AssetExecDto> Assets { get; set; } = Enumerable.Empty<AssetExecDto>();
    }

    // DTO para cada ativo retornado na execução do prompt
    public class AssetExecDto
    {
        public string Ticker { get; set; } = "";
        public double ROE { get; set; }
        public double DY { get; set; }
        public double CAGR { get; set; }
        public double Liquidez { get; set; }
        public string Risco { get; set; } = "";
        public double Score { get; set; }
    }

    public class FullFlowTestServerParametrizedTests
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FullFlowTestServerParametrizedTests()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Program>() // mantém a configuração da API
                .ConfigureServices(services =>
                {
                    // DbContext InMemory
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<InvestScoreDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<InvestScoreDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestServerParametrizedDb");
                    });

                    // Injeta IHttpClientFactory fake (OpenAI simulada)
                    var httpClientFactoryDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IHttpClientFactory));
                    if (httpClientFactoryDescriptor != null)
                        services.Remove(httpClientFactoryDescriptor);

                    services.AddSingleton<IHttpClientFactory, HttpClientFactoryFake>();
                });

            var server = new TestServer(builder);
            _client = server.CreateClient();
        }

        [Theory]
        [InlineData("AAPL")]
        [InlineData("MSFT")]
        [InlineData("TSLA")]
        public async Task FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets(string expectedTicker)
        {
            // 1) Cria FileRecord
            var fileRecordDto = new FileRecordCreateDto
            {
                FilePath = "ativos.txt",
                Title = $"Ativos {expectedTicker}",
                Description = "Fluxo parametrizado com TestServer + Fake"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/filerecords", fileRecordDto);
            createResponse.EnsureSuccessStatusCode();

            var fileRecord = await createResponse.Content.ReadFromJsonAsync<FileRecordResponseDto>(_jsonOptions);
            fileRecord.Should().NotBeNull();

            // 2) Executa Prompt
            var executeResponse = await _client.PostAsync($"/api/prompts/{fileRecord!.Id}/execute", null);
            executeResponse.EnsureSuccessStatusCode();

            var execPayload = await executeResponse.Content.ReadFromJsonAsync<ExecutePromptResponseDto>(_jsonOptions);
            execPayload.Should().NotBeNull();
            execPayload!.Assets.Should().NotBeEmpty();
            execPayload.Assets.Select(a => a.Ticker).Should().Contain(expectedTicker);

            // 3) Consulta ativos
            var assetsResponse = await _client.GetAsync("/api/assets");
            assetsResponse.EnsureSuccessStatusCode();

            var assets = await assetsResponse.Content.ReadFromJsonAsync<IEnumerable<AssetResponseDto>>(_jsonOptions);
            assets.Should().NotBeEmpty();
            assets!.Select(a => a.Ticker).Should().Contain(expectedTicker);
        }
    }
}
