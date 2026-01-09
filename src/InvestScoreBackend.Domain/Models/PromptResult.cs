using System.Collections.Generic;

namespace InvestScoreBackend.Domain.Models
{
    public class PromptResult
    {
        public List<PromptAssetResult> Assets { get; set; } = new();
    }

    public class PromptAssetResult
    {
        public string Ticker { get; set; } = string.Empty;
        public double ROE { get; set; }
        public double DY { get; set; }
        public double CAGR { get; set; }
        public double Liquidez { get; set; }
        public string Risco { get; set; } = string.Empty; // precisa ser string
        public double Score { get; set; }
    }

}
