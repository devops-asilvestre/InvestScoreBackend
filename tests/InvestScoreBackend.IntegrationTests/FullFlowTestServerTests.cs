using System.Net.Http.Json;
using InvestScoreBackend.Infrastructure.Persistence;
using InvestScoreBackend.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;

namespace InvestScoreBackend.IntegrationTests
{
    public class FullFlowTestServerTests
    {
        private readonly HttpClient _client;

        public FullFlowTestServerTests()
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
                        options.UseInMemoryDatabase("TestServerDb");
                    });

                    // Aqui você pode substituir IHttpClientFactory por um fake, se necessário
                    // services.AddSingleton<IHttpClientFactory, HttpClientFactoryFake>();
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
                Description = "Fluxo completo com TestServer"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/filerecords", fileRecordDto);
            createResponse.EnsureSuccessStatusCode();

            var fileRecord = await createResponse.Content.ReadFromJsonAsync<FileRecordResponseDto>();
            fileRecord.Should().NotBeNull();

            // 2. Executar Prompt
            var executeResponse = await _client.PostAsync($"/api/prompts/{fileRecord.Id}/execute", null);
            executeResponse.EnsureSuccessStatusCode();

            var executeJson = await executeResponse.Content.ReadFromJsonAsync<dynamic>();
            ((IEnumerable<object>)executeJson.assets).Should().NotBeEmpty();

            // 3. Consultar ativos
            var assetsResponse = await _client.GetAsync("/api/assets");
            assetsResponse.EnsureSuccessStatusCode();

            var assets = await assetsResponse.Content.ReadFromJsonAsync<IEnumerable<AssetResponseDto>>();
            assets.Should().NotBeEmpty();
        }
    }
}
