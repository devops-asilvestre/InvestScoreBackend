using InvestScoreBackend.Domain.Models;
using InvestScoreBackend.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvestScoreBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromptsController : ControllerBase
    {
        private readonly IPromptService _promptService;

        public PromptsController(IPromptService promptService)
        {
            _promptService = promptService;
        }

        // POST: api/prompts/{id}/execute
        [HttpPost("{id}/execute")]
        public async Task<IActionResult> ExecutePrompt(int id)
        {
            try
            {
                // 🔎 Executa o prompt via OpenAI e retorna os ativos criados
                var assets = await _promptService.ExecutePromptAsync(id);

                var response = new
                {
                    message = $"Prompt {id} executado com sucesso.",
                    assets = assets.Select(a => new AssetResponseDto
                    {
                        Ticker = a.Ticker,
                        ROE = a.ROE,
                        DY = a.DY,
                        CAGR = a.CAGR,
                        Liquidez = a.Liquidez,
                        Risco = a.Risco,
                        Score = a.Score
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
