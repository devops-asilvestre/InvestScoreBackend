using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvestScoreBackend.Infrastructure.Repositories
{
    public class FileRecordRepository : IFileRecordRepository
    {
        private readonly InvestScoreDbContext _context;

        public FileRecordRepository(InvestScoreDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todos os registros de arquivos
        /// </summary>
        public async Task<IEnumerable<FileRecord>> GetAllAsync()
        {
            return await _context.Set<FileRecord>().ToListAsync();
        }

        /// <summary>
        /// Retorna um registro de arquivo pelo Id
        /// </summary>
        public async Task<FileRecord?> GetByIdAsync(int id)
        {
            return await _context.Set<FileRecord>().FindAsync(id);
        }

        /// <summary>
        /// Adiciona um novo registro de arquivo
        /// </summary>
        public async Task<FileRecord> AddAsync(FileRecord fileRecord)
        {
            await _context.Set<FileRecord>().AddAsync(fileRecord);
            await _context.SaveChangesAsync();
            return fileRecord;
        }

        /// <summary>
        /// Atualiza um registro de arquivo existente
        /// </summary>
        public async Task<FileRecord> UpdateAsync(FileRecord fileRecord)
        {
            _context.Set<FileRecord>().Update(fileRecord);
            await _context.SaveChangesAsync();
            return fileRecord;
        }

        /// <summary>
        /// Remove um registro de arquivo pelo Id
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<FileRecord>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"FileRecord com Id {id} não encontrado.");
            }
        }
    }
}
