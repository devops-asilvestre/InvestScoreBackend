using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvestScoreBackend.Infrastructure.Repositories
{
    public class AssetHeadRepository : IAssetHeadRepository
    {
        private readonly InvestScoreDbContext _context;

        public AssetHeadRepository(InvestScoreDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todos os cabeçalhos de ativos
        /// </summary>
        public async Task<IEnumerable<AssetHead>> GetAllAsync()
        {
            return await _context.Set<AssetHead>().ToListAsync();
        }

        /// <summary>
        /// Retorna um cabeçalho de ativo pelo Id
        /// </summary>
        public async Task<AssetHead?> GetByIdAsync(int id)
        {
            return await _context.Set<AssetHead>().FindAsync(id);
        }

        /// <summary>
        /// Adiciona um novo cabeçalho de ativo
        /// </summary>
        public async Task<AssetHead> AddAsync(AssetHead assetHead)
        {
            await _context.Set<AssetHead>().AddAsync(assetHead);
            await _context.SaveChangesAsync();
            return assetHead;
        }

        /// <summary>
        /// Atualiza um cabeçalho de ativo existente
        /// </summary>
        public async Task<AssetHead> UpdateAsync(AssetHead assetHead)
        {
            _context.Set<AssetHead>().Update(assetHead);
            await _context.SaveChangesAsync();
            return assetHead;
        }

        /// <summary>
        /// Remove um cabeçalho de ativo pelo Id
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<AssetHead>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"AssetHead com Id {id} não encontrado.");
            }
        }
    }
}
