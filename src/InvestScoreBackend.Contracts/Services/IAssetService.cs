using InvestScoreBackend.Domain.Entities;

namespace InvestScoreBackend.Domain.Interfaces.Services
{
    public interface IAssetService
    {
        Task<Asset?> GetAssetAsync(int id);
        Task<IEnumerable<Asset>> GetAllAssetsAsync();
        Task<Asset> CreateAssetAsync(Asset asset);
        Task<Asset> UpdateAssetAsync(Asset asset);
        Task DeleteAssetAsync(int id);
    }

    public interface IAssetHeadService
    {
        Task<AssetHead?> GetAssetHeadAsync(int id);
        Task<IEnumerable<AssetHead>> GetAllAssetHeadsAsync();
        Task<AssetHead> CreateAssetHeadAsync(AssetHead assetHead);
        Task<AssetHead> UpdateAssetHeadAsync(AssetHead assetHead);
        Task DeleteAssetHeadAsync(int id);
    }
}
