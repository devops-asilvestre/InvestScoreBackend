using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Models;

namespace InvestScoreBackend.Application.Services
{
    public class FileRecordService : IFileRecordService
    {
        private readonly IFileRecordRepository _repository;

        public FileRecordService(IFileRecordRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Salva um arquivo físico no banco como FileRecord
        /// </summary>
        public async Task<FileRecord> SaveFileAsync(string filePath, string title, string description)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado", filePath);

            var content = await File.ReadAllTextAsync(filePath);

            var record = new FileRecord
            {
                FilePath = filePath,
                Title = title,
                Description = description,
                Content = content,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(record);
            return record;
        }

        /// <summary>
        /// Retorna um FileRecord pelo Id
        /// </summary>
        public async Task<FileRecord?> GetFileRecordAsync(int id) =>
            await _repository.GetByIdAsync(id);

        /// <summary>
        /// Retorna todos os FileRecords
        /// </summary>
        public async Task<IEnumerable<FileRecord>> GetAllFileRecordsAsync() =>
            await _repository.GetAllAsync();

        /// <summary>
        /// Cria um novo FileRecord
        /// </summary>
        public async Task<FileRecord> CreateFileRecordAsync(FileRecordCreateDto fileRecord)
        {
            if (string.IsNullOrWhiteSpace(fileRecord.FilePath))
                throw new ArgumentException("Caminho do arquivo não pode ser vazio.");

            if (!File.Exists(fileRecord.FilePath))
                throw new FileNotFoundException("Arquivo não encontrado.", fileRecord.FilePath);

            // Lê o conteúdo do arquivo físico

            var created = await _repository.AddAsync(new FileRecord
            {
                Content = await File.ReadAllTextAsync(fileRecord.FilePath),
                CreatedAt = DateTime.UtcNow,
                Description = fileRecord.Description,
                FilePath = fileRecord.FilePath,
                IsAvailable = true,
                Title = fileRecord.Title
            });
            return created;
        }


        /// <summary>
        /// Remove um FileRecord pelo Id
        /// </summary>
        public async Task DeleteFileRecordAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
