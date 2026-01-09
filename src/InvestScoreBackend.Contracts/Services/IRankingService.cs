namespace InvestScoreBackend.Contracts.Services
{
    using InvestScoreBackend.Domain.Entities;
    using System.Collections.Generic;

    public interface IRankingService
    {
        IEnumerable<Asset> CalculateRanking(IEnumerable<Asset> assets, string perfil, int topN);
    }
}
