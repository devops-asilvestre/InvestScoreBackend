using InvestScoreBackend.Domain.Entities;

namespace InvestScoreBackend.Contracts.Services
{
    public interface IPromptService
    {
        /// <summary>
        /// Executa um prompt salvo no banco via OpenAI e retorna os ativos criados.
        /// </summary>
        /// <param name="fileRecordId">ID do registro de prompt salvo no banco</param>
        /// <returns>Lista de ativos gerados a partir da resposta da IA</returns>
        Task<List<Asset>> ExecutePromptAsync(int fileRecordId);

        /// <summary>
        /// Executa um prompt específico para um único ticker via OpenAI.
        /// </summary>
        /// <param name="ticker">Código do ativo (ex: PETR4.SA ou AAPL)</param>
        /// <returns>Ativo gerado a partir da resposta da IA</returns>
        Task<Asset> FetchAssetFromAlphaVantageAsync(string ticker);
    }
}
