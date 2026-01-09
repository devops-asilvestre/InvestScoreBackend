// src/InvestScoreBackend.Contracts/Repositories/IAssetRepository.cs
using InvestScoreBackend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvestScoreBackend.Contracts.Repositories
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Asset>> GetAllAsync();
        Task<Asset?> GetByIdAsync(int id);
        Task<Asset> AddAsync(Asset asset);
        Task<Asset> UpdateAsync(Asset asset);
        Task DeleteAsync(int id);
    }

    public interface IAssetHeadRepository { 
        Task<AssetHead?> GetByIdAsync(int id); 
        Task<IEnumerable<AssetHead>> GetAllAsync();
        Task<AssetHead> AddAsync(AssetHead assetHead);
        Task<AssetHead> UpdateAsync(AssetHead assetHead); 
        Task DeleteAsync(int id); 
    }
}
