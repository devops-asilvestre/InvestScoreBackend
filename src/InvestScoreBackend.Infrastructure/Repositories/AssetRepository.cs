using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvestScoreBackend.Infrastructure.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly InvestScoreDbContext _context;

        public AssetRepository(InvestScoreDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Retorna todos os ativos
        /// </summary>
        public async Task<IEnumerable<Asset>> GetAllAsync()
        {
            return await _context.Set<Asset>().ToListAsync();
        }

        /// <summary>
        /// Retorna um ativo pelo Id
        /// </summary>
        public async Task<Asset?> GetByIdAsync(int id)
        {
            return await _context.Set<Asset>().FindAsync(id);
        }

        /// <summary>
        /// Adiciona um novo ativo
        /// </summary>
        public async Task<Asset> AddAsync(Asset asset)
        {
            await _context.Set<Asset>().AddAsync(asset);
            await _context.SaveChangesAsync();
            return asset;
        }

        /// <summary>
        /// Atualiza um ativo existente
        /// </summary>
        public async Task<Asset> UpdateAsync(Asset asset)
        {
            _context.Set<Asset>().Update(asset);
            await _context.SaveChangesAsync();
            return asset;
        }

        /// <summary>
        /// Remove um ativo pelo Id
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<Asset>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Asset com Id {id} não encontrado.");
            }
        }


    }
}
