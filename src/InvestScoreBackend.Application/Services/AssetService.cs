using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Interfaces.Services;

namespace InvestScoreBackend.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;

        public AssetService(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        /// <summary>
        /// Busca um ativo pelo Id
        /// </summary>
        public async Task<Asset?> GetAssetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido para busca de Asset.");

            return await _assetRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Retorna todos os ativos
        /// </summary>
        public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
        {
            var assets = await _assetRepository.GetAllAsync();
            return assets ?? Enumerable.Empty<Asset>();
        }

        /// <summary>
        /// Cria um novo ativo
        /// </summary>
        public async Task<Asset> CreateAssetAsync(Asset asset)
        {
            ValidateAsset(asset);
            await _assetRepository.AddAsync(asset);
            return asset;
        }

        /// <summary>
        /// Atualiza um ativo existente
        /// </summary>
        public async Task<Asset> UpdateAssetAsync(Asset asset)
        {
            if (asset.Id <= 0)
                throw new ArgumentException("Id inválido para atualização de Asset.");

            ValidateAsset(asset);

            await _assetRepository.UpdateAsync(asset);
            return asset;
        }

        /// <summary>
        /// Remove um ativo pelo Id
        /// </summary>
        public async Task DeleteAssetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido para exclusão de Asset.");

            var existing = await _assetRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Ativo com Id {id} não encontrado.");

            await _assetRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Valida os dados do ativo
        /// </summary>
        private static void ValidateAsset(Asset asset)
        {
            if (string.IsNullOrWhiteSpace(asset.Ticker))
                throw new ArgumentException("Ticker do ativo não pode ser vazio.");

            if (asset.ROE < 0 || asset.DY < 0 || asset.CAGR < 0 || asset.Liquidez < 0 || asset.Score < 0)
                throw new ArgumentException("Indicadores financeiros não podem ser negativos.");
        }
    }
}
