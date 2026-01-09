using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Interfaces.Services;
using InvestScoreBackend.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InvestScoreBackend.Application.Services
{
    public class PromptService : IPromptService
    {
        private readonly IFileRecordRepository _fileRecordRepository;
        private readonly IAssetService _assetService;
        private readonly IAssetHeadService _assetHeadService;
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;

        public PromptService(
            IFileRecordRepository fileRecordRepository,
            IAssetService assetService,
            IAssetHeadService assetHeadService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _fileRecordRepository = fileRecordRepository;
            _assetService = assetService;
            _assetHeadService = assetHeadService;
            _httpClient = httpClientFactory.CreateClient();
            _openAiApiKey = configuration["OpenAI:ApiKey"];
        }

        /// <summary>
        /// Executa um prompt salvo no banco via OpenAI e popula a tabela Assets vinculados a um AssetHead
        /// </summary>
        public async Task<List<Asset>> ExecutePromptAsync(int fileRecordId)
        {
            var fileRecord = await _fileRecordRepository.GetByIdAsync(fileRecordId);
            if (fileRecord == null || !fileRecord.IsAvailable)
                throw new InvalidOperationException("Prompt não encontrado ou indisponível.");

            // Cria cabeçalho de processamento
            var assetHead = new AssetHead
            {
                FileRecordId = fileRecord.Id,
                FileRecord = fileRecord,
                ProcessedAt = DateTime.UtcNow,
                MachineIp = "127.0.0.1" // pode ser obtido dinamicamente
            };
            await _assetHeadService.CreateAssetHeadAsync(assetHead);

            // Chama OpenAI
            var result = await CallOpenAiAsync(fileRecord.Content);

            if (result == null || result.Assets.Count == 0)
                throw new Exception("OpenAI não retornou ativos válidos.");

            // Converte para entidades Asset vinculadas ao AssetHead
            var assets = result.Assets.Select(a => new Asset
            {
                Ticker = a.Ticker,
                ROE = a.ROE,
                DY = a.DY,
                CAGR = a.CAGR,
                Liquidez = a.Liquidez,
                Risco = a.Risco,
                Score = a.Score,
                AssetHeadId = assetHead.Id
            }).ToList();

            // Persiste no banco
            foreach (var asset in assets)
                await _assetService.CreateAssetAsync(asset);

            return assets;
        }

        /// <summary>
        /// Busca um único ativo via prompt (atalho)
        /// </summary>
        public async Task<Asset> FetchAssetFromAlphaVantageAsync(string ticker)
        {
            var promptText = $"Analise o ativo {ticker} e retorne ROE, DY, CAGR, Liquidez, Risco e Score em JSON.";
            var result = await CallOpenAiAsync(promptText);

            if (result == null || result.Assets.Count == 0)
                throw new Exception("OpenAI não retornou ativo válido.");

            var assetResult = result.Assets.First();

            var asset = new Asset
            {
                Ticker = assetResult.Ticker,
                ROE = assetResult.ROE,
                DY = assetResult.DY,
                CAGR = assetResult.CAGR,
                Liquidez = assetResult.Liquidez,
                Risco = assetResult.Risco,
                Score = assetResult.Score
            };

            await _assetService.CreateAssetAsync(asset);
            return asset;
        }

        /// <summary>
        /// Método privado para chamada ao OpenAI e desserialização
        /// </summary>
        private async Task<PromptResult?> CallOpenAiAsync(string promptText)
        {
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[] {
                    new { role = "system", content = "Você é um analista financeiro. Retorne sempre em JSON no formato { assets: [...] }" },
                    new { role = "user", content = promptText }
                },
                temperature = 0.2
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao chamar OpenAI: {response.StatusCode}");

            var jsonString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonString);
            var messageContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            messageContent = messageContent?.Trim() ?? string.Empty;

            if (messageContent.StartsWith("```"))
            {
                var start = messageContent.IndexOf('{');
                var end = messageContent.LastIndexOf('}');
                messageContent = messageContent.Substring(start, end - start + 1);
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<PromptResult>(messageContent, jsonOptions);
        }
    }
}
