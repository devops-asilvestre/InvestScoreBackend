using InvestScoreBackend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvestScoreBackend.Contracts.Repositories
{
    public interface IFileRecordRepository
    {
        Task<IEnumerable<FileRecord>> GetAllAsync();
        Task<FileRecord?> GetByIdAsync(int id);
        Task<FileRecord> AddAsync(FileRecord fileRecord);
        Task<FileRecord> UpdateAsync(FileRecord fileRecord);
        Task DeleteAsync(int id);
    }
}
