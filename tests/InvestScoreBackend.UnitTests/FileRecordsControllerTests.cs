using InvestScoreBackend.API.Controllers;
using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;

namespace InvestScoreBackend.UnitTests.Controllers
{
    public class FileRecordsControllerTests
    {
        private readonly Mock<IFileRecordService> _fileRecordServiceMock;
        private readonly FileRecordsController _controller;

        public FileRecordsControllerTests()
        {
            _fileRecordServiceMock = new Mock<IFileRecordService>();
            _controller = new FileRecordsController(_fileRecordServiceMock.Object);
        }

        [Fact]
        public async Task GetFileRecords_Should_Return_List()
        {
            var records = new List<FileRecord>
            {
                new FileRecord { Id = 1, Title = "Ativos", FilePath = "ativos.txt", Content = "PETR4.SA", IsAvailable = true }
            };
            _fileRecordServiceMock.Setup(s => s.GetAllFileRecordsAsync()).ReturnsAsync(records);

            var result = await _controller.GetFileRecords();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as IEnumerable<FileRecordResponseDto>;
            response.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetFileRecord_Should_Return_Record_When_Found()
        {
            var record = new FileRecord { Id = 1, Title = "Ativos", FilePath = "ativos.txt" };
            _fileRecordServiceMock.Setup(s => s.GetFileRecordAsync(1)).ReturnsAsync(record);

            var result = await _controller.GetFileRecord(1);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult.Value as FileRecordResponseDto;
            response.Title.Should().Be("Ativos");
        }

        [Fact]
        public async Task GetFileRecord_Should_Return_NotFound_When_NotFound()
        {
            _fileRecordServiceMock.Setup(s => s.GetFileRecordAsync(1)).ReturnsAsync((FileRecord)null);

            var result = await _controller.GetFileRecord(1);

            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            ((dynamic)notFoundResult.Value).message.Should().Be("Registro de arquivo não encontrado.");
        }

        [Fact]
        public async Task CreateFileRecord_Should_Return_Created_Record()
        {
            var dto = new FileRecordCreateDto { FilePath = "ativos.txt", Title = "Ativos", Description = "Teste" };
            var record = new FileRecord { Id = 1, FilePath = "ativos.txt", Title = "Ativos", Description = "Teste" };

            _fileRecordServiceMock.Setup(s => s.CreateFileRecordAsync(dto)).ReturnsAsync(record);

            var result = await _controller.CreateFileRecord(dto);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            var response = createdResult.Value as FileRecordResponseDto;
            response.Title.Should().Be("Ativos");
        }

        [Fact]
        public async Task DeleteFileRecord_Should_Return_NoContent_When_Success()
        {
            _fileRecordServiceMock.Setup(s => s.DeleteFileRecordAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteFileRecord(1);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
