namespace InvestScoreBackend.Domain.Entities
{
    public class FileRecord
    {
        public int Id { get; set; }

        // Caminho físico ou nome do arquivo original
        public string FilePath { get; set; } = string.Empty;

        // Título amigável do arquivo
        public string Title { get; set; } = string.Empty;

        // Descrição do conteúdo do arquivo
        public string Description { get; set; } = string.Empty;

        // Conteúdo do arquivo .txt armazenado no banco
        public string Content { get; set; } = string.Empty;

        // Flag para indicar se o arquivo está disponível para uso
        public bool IsAvailable { get; set; } = true;

        // Data de criação do registro
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
