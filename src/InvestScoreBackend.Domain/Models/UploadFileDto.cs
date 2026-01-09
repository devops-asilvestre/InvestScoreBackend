namespace InvestScoreBackend.Domain.Models
{
    /// <summary>
    /// DTO usado para upload inicial de arquivos
    /// </summary>
    public class UploadFileDto
    {
        public string FilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO usado para criação de registros de arquivo
    /// </summary>
    public class FileRecordCreateDto
    {
        public string FilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO usado para atualização de registros de arquivo
    /// </summary>
    public class FileRecordUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }

    /// <summary>
    /// DTO usado para retorno de registros de arquivo
    /// </summary>
    public class FileRecordResponseDto
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
