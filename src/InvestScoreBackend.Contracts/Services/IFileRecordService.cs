using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Models;

namespace InvestScoreBackend.Contracts.Services
{
    public interface IFileRecordService
    {
        Task<FileRecord?> GetFileRecordAsync(int id);
        Task<IEnumerable<FileRecord>> GetAllFileRecordsAsync();
        Task<FileRecord> CreateFileRecordAsync(FileRecordCreateDto fileRecord);
        Task DeleteFileRecordAsync(int id);
    }
}
