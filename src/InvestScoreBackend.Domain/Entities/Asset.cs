namespace InvestScoreBackend.Domain.Entities
{
    public class AssetHead
    {
        public int Id { get; set; }

        // Relacionamento com FileRecord (origem do processamento)
        public int FileRecordId { get; set; }
        public FileRecord FileRecord { get; set; }

        // Metadados do processamento
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public string MachineIp { get; set; } = string.Empty;

        // Lista de ativos vinculados a este cabeçalho
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }

    public class Asset
    {
        public int Id { get; set; } // EF gera automaticamente
        public string Ticker { get; set; } = string.Empty;
        public double ROE { get; set; }
        public double DY { get; set; }
        public double CAGR { get; set; }
        public double Liquidez { get; set; }
        public string Risco { get; set; }
        public double Score { get; set; }

        // Relacionamento com AssetHead
        public int AssetHeadId { get; set; }
        public AssetHead AssetHead { get; set; }
    }
}
