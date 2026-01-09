using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Interfaces.Services;

namespace InvestScoreBackend.Application.Services
{
    public class AssetHeadService : IAssetHeadService
    {
        private readonly IAssetHeadRepository _repository;

        public AssetHeadService(IAssetHeadRepository repository)
        {
            _repository = repository;
        }

        public async Task<AssetHead?> GetAssetHeadAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido para busca de AssetHead.");

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AssetHead>> GetAllAssetHeadsAsync()
        {
            var heads = await _repository.GetAllAsync();
            return heads ?? Enumerable.Empty<AssetHead>();
        }

        public async Task<AssetHead> CreateAssetHeadAsync(AssetHead assetHead)
        {
            assetHead.ProcessedAt = DateTime.UtcNow;
            await _repository.AddAsync(assetHead);
            return assetHead;
        }

        public async Task<AssetHead> UpdateAssetHeadAsync(AssetHead assetHead)
        {
            if (assetHead.Id <= 0)
                throw new ArgumentException("Id inválido para atualização de AssetHead.");

            var updated = await _repository.UpdateAsync(assetHead);
            return updated;
        }

        public async Task DeleteAssetHeadAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido para exclusão de AssetHead.");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"AssetHead com Id {id} não encontrado.");

            await _repository.DeleteAsync(id);
        }
    }
}
