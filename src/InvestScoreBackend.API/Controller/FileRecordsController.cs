using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvestScoreBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileRecordsController : ControllerBase
    {
        private readonly IFileRecordService _fileRecordService;

        public FileRecordsController(IFileRecordService fileRecordService)
        {
            _fileRecordService = fileRecordService;
        }

        // GET: api/filerecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileRecordResponseDto>>> GetFileRecords()
        {
            var records = await _fileRecordService.GetAllFileRecordsAsync();

            var response = records.Select(r => new FileRecordResponseDto
            {
                Id = r.Id,
                FilePath = r.FilePath,
                Title = r.Title,
                Description = r.Description,
                Content = r.Content,
                IsAvailable = r.IsAvailable,
                CreatedAt = r.CreatedAt
            });

            return Ok(response);
        }

        // GET: api/filerecords/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FileRecordResponseDto>> GetFileRecord(int id)
        {
            var record = await _fileRecordService.GetFileRecordAsync(id);
            if (record == null) return NotFound(new { message = "Registro de arquivo não encontrado." });

            var response = new FileRecordResponseDto
            {
                Id = record.Id,
                FilePath = record.FilePath,
                Title = record.Title,
                Description = record.Description,
                Content = record.Content,
                IsAvailable = record.IsAvailable,
                CreatedAt = record.CreatedAt
            };

            return Ok(response);
        }

        // POST: api/filerecords
        [HttpPost]
        public async Task<ActionResult<FileRecordResponseDto>> CreateFileRecord([FromBody] FileRecordCreateDto dto)
        {
            try
            {

                var created = await _fileRecordService.CreateFileRecordAsync(dto);

                var response = new FileRecordResponseDto
                {
                    Id = created.Id,
                    FilePath = created.FilePath,
                    Title = created.Title,
                    Description = created.Description,
                    Content = created.Content,
                    IsAvailable = created.IsAvailable,
                    CreatedAt = created.CreatedAt
                };

                return CreatedAtAction(nameof(GetFileRecord), new { id = created.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/filerecords/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileRecord(int id)
        {
            try
            {
                await _fileRecordService.DeleteFileRecordAsync(id);
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
    }
}
