using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Interfaces.Services;
using InvestScoreBackend.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvestScoreBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IPromptService _promptService;

        public AssetsController(IAssetService assetService, IPromptService promptService)
        {
            _assetService = assetService;
            _promptService = promptService;
        }

        // GET: api/assets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetResponseDto>>> GetAssets()
        {
            var assets = await _assetService.GetAllAssetsAsync();

            var response = assets.Select(a => new AssetResponseDto
            {
                Id = a.Id,
                Ticker = a.Ticker,
                ROE = a.ROE,
                DY = a.DY,
                CAGR = a.CAGR,
                Liquidez = a.Liquidez,
                Risco = a.Risco,
                Score = a.Score,
                AssetHeadId = a.AssetHeadId
            });

            return Ok(response);
        }

        // GET: api/assets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetResponseDto>> GetAsset(int id)
        {
            var asset = await _assetService.GetAssetAsync(id);
            if (asset == null) return NotFound(new { message = "Ativo não encontrado." });

            var response = new AssetResponseDto
            {
                Id = asset.Id,
                Ticker = asset.Ticker,
                ROE = asset.ROE,
                DY = asset.DY,
                CAGR = asset.CAGR,
                Liquidez = asset.Liquidez,
                Risco = asset.Risco,
                Score = asset.Score,
                AssetHeadId = asset.AssetHeadId
            };

            return Ok(response);
        }

        // POST: api/assets
        // Cria um ativo automaticamente usando PromptService
        [HttpPost]
        public async Task<ActionResult<AssetResponseDto>> CreateAsset([FromBody] AssetCreateDto dto)
        {
            try
            {
                var asset = await _promptService.FetchAssetFromAlphaVantageAsync(dto.Ticker);
                var created = await _assetService.CreateAssetAsync(asset);

                var response = new AssetResponseDto
                {
                    Id = created.Id,
                    Ticker = created.Ticker,
                    ROE = created.ROE,
                    DY = created.DY,
                    CAGR = created.CAGR,
                    Liquidez = created.Liquidez,
                    Risco = created.Risco,
                    Score = created.Score,
                    AssetHeadId = created.AssetHeadId
                };

                return CreatedAtAction(nameof(GetAsset), new { id = created.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/assets/import
        // Importa um ativo a partir de um ticker informado
        [HttpPost("import")]
        public async Task<ActionResult<AssetResponseDto>> ImportAsset([FromBody] AssetImportDto dto)
        {
            try
            {
                var asset = await _promptService.FetchAssetFromAlphaVantageAsync(dto.Ticker);
                var created = await _assetService.CreateAssetAsync(asset);

                var response = new AssetResponseDto
                {
                    Id = created.Id,
                    Ticker = created.Ticker,
                    ROE = created.ROE,
                    DY = created.DY,
                    CAGR = created.CAGR,
                    Liquidez = created.Liquidez,
                    Risco = created.Risco,
                    Score = created.Score,
                    AssetHeadId = created.AssetHeadId
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/assets/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            try
            {
                await _assetService.DeleteAssetAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/assets/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<AssetResponseDto>> UpdateAsset(int id, [FromBody] AssetCreateDto dto)
        {
            try
            {
                var existing = await _assetService.GetAssetAsync(id);
                if (existing == null) return NotFound(new { message = "Ativo não encontrado." });

                existing.ROE = dto.ROE;
                existing.DY = dto.DY;
                existing.CAGR = dto.CAGR;
                existing.Liquidez = dto.Liquidez;
                existing.Risco = dto.Risco;
                existing.Score = dto.Score;

                var updated = await _assetService.UpdateAssetAsync(existing);

                var response = new AssetResponseDto
                {
                    Id = updated.Id,
                    Ticker = updated.Ticker,
                    ROE = updated.ROE,
                    DY = updated.DY,
                    CAGR = updated.CAGR,
                    Liquidez = updated.Liquidez,
                    Risco = updated.Risco,
                    Score = updated.Score,
                    AssetHeadId = updated.AssetHeadId
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
