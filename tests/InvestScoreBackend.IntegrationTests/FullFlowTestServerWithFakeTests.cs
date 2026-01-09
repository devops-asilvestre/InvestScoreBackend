using System.Net.Http.Json;
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
    public class FullFlowTestServerWithFakeTests
    {
        private readonly HttpClient _client;

        public FullFlowTestServerWithFakeTests()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Program>() // Usa a mesma configuração da API
                .ConfigureServices(services =>
                {
                    // Substitui DbContext por InMemory
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<InvestScoreDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<InvestScoreDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestServerFakeDb");
                    });

                    // Substitui IHttpClientFactory pelo fake
                    var httpClientFactoryDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IHttpClientFactory));
                    if (httpClientFactoryDescriptor != null)
                        services.Remove(httpClientFactoryDescriptor);

                    services.AddSingleton<IHttpClientFactory, HttpClientFactoryFake>();
                });

            var server = new TestServer(builder);
            _client = server.CreateClient();
        }

        [Fact]
        public async Task FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets()
        {
            // 1. Criar FileRecord
            var fileRecordDto = new FileRecordCreateDto
            {
                FilePath = "ativos.txt",
                Title = "Ativos B3",
                Description = "Fluxo completo com TestServer + Fake"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/filerecords", fileRecordDto);
            createResponse.EnsureSuccessStatusCode();

            var fileRecord = await createResponse.Content.ReadFromJsonAsync<FileRecordResponseDto>();
            fileRecord.Should().NotBeNull();

            // 2. Executar Prompt
            var executeResponse = await _client.PostAsync($"/api/prompts/{fileRecord.Id}/execute", null);
            executeResponse.EnsureSuccessStatusCode();

            var executeJson = await executeResponse.Content.ReadFromJsonAsync<dynamic>();
            ((IEnumerable<object>)executeJson.assets).Should().HaveCount(3);

            // 3. Consultar ativos
            var assetsResponse = await _client.GetAsync("/api/assets");
            assetsResponse.EnsureSuccessStatusCode();

            var assets = await assetsResponse.Content.ReadFromJsonAsync<IEnumerable<AssetResponseDto>>();
            assets.Should().NotBeEmpty();
            assets.Select(a => a.Ticker).Should().Contain(new[] { "AAPL", "MSFT", "TSLA" });
        }
    }
}
