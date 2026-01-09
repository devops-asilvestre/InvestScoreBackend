namespace InvestScoreBackend.Domain.Models
{
    /// <summary>
    /// DTO usado para importação de ativos via /api/assets/import
    /// </summary>
    public class AssetImportDto
    {
        public string Ticker { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO usado para criação de ativos via POST /api/assets
    /// </summary>
    public class AssetCreateDto
    {
        public string Ticker { get; set; } = string.Empty;
        public double ROE { get; set; }
        public double DY { get; set; }
        public double CAGR { get; set; }
        public double Liquidez { get; set; }
        public string Risco { get; set; } = string.Empty;
        public double Score { get; set; }
    }

    /// <summary>
    /// DTO usado para representar o cabeçalho de processamento de ativos
    /// </summary>
    public class AssetHeadDto
    {
        public int FileRecordId { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public string MachineIp { get; set; } = string.Empty;

        // Lista de ativos processados
        public List<AssetResponseDto> Assets { get; set; } = new();
    }

    /// <summary>
    /// DTO usado para retornar ativos nos endpoints GET
    /// </summary>
    public class AssetResponseDto
    {
        public int Id { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public double ROE { get; set; }
        public double DY { get; set; }
        public double CAGR { get; set; }
        public double Liquidez { get; set; }
        public string Risco { get; set; } = string.Empty;
        public double Score { get; set; }
        public int AssetHeadId { get; set; } // vínculo com cabeçalho
    }
}
