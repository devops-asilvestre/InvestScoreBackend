using InvestScoreBackend.API.Controllers;
using InvestScoreBackend.Application.Services;
using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Entities;
using InvestScoreBackend.Domain.Models;
using InvestScoreBackend.Infrastructure.Persistence;
using InvestScoreBackend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace InvestScoreBackend.IntegrationTests.Controllers
{
    public class FileRecordsControllerIntegrationTests
    {
        private readonly InvestScoreDbContext _dbContext;
        private readonly IFileRecordRepository _fileRecordRepo;
        private readonly IFileRecordService _fileRecordService;
        private readonly FileRecordsController _controller;

        public FileRecordsControllerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<InvestScoreDbContext>()
                .UseInMemoryDatabase("FileRecordsIntegrationDb")
                .Options;

            _dbContext = new InvestScoreDbContext(options);
            _fileRecordRepo = new FileRecordRepository(_dbContext);
            _fileRecordService = new FileRecordService(_fileRecordRepo);
            _controller = new FileRecordsController(_fileRecordService);
        }

        // POST: api/filerecords
        [Fact]
        public async Task CreateFileRecord_Should_Persist_Record()
        {
            var dto = new FileRecordCreateDto
            {
                FilePath = "ativos.txt",
                Title = "Ativos B3",
                Description = "Arquivo de teste"
            };

            var result = await _controller.CreateFileRecord(dto);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();

            var response = createdResult.Value as FileRecordResponseDto;
            response.Title.Should().Be("Ativos B3");

            _dbContext.FileRecords.Any(r => r.Title == "Ativos B3").Should().BeTrue();
        }

        // GET: api/filerecords
        [Fact]
        public async Task GetFileRecords_Should_Return_All_Records()
        {
            _dbContext.FileRecords.Add(new FileRecord
            {
                FilePath = "ativos.txt",
                Title = "Ativos",
                Description = "Teste",
                Content = "PETR4.SA",
                IsAvailable = true
            });
            _dbContext.SaveChanges();

            var result = await _controller.GetFileRecords();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as IEnumerable<FileRecordResponseDto>;
            response.Should().ContainSingle(r => r.Title == "Ativos");
        }

        // GET: api/filerecords/{id}
        [Fact]
        public async Task GetFileRecord_Should_Return_Record_When_Found()
        {
            var record = new FileRecord
            {
                FilePath = "ativos.txt",
                Title = "Ativos",
                Description = "Teste",
                Content = "VALE3.SA",
                IsAvailable = true
            };
            _dbContext.FileRecords.Add(record);
            _dbContext.SaveChanges();

            var result = await _controller.GetFileRecord(record.Id);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as FileRecordResponseDto;
            response.Title.Should().Be("Ativos");
        }

        // DELETE: api/filerecords/{id}
        [Fact]
        public async Task DeleteFileRecord_Should_Remove_Record()
        {
            var record = new FileRecord
            {
                FilePath = "ativos.txt",
                Title = "Ativos",
                Description = "Teste",
                Content = "ITUB4.SA",
                IsAvailable = true
            };
            _dbContext.FileRecords.Add(record);
            _dbContext.SaveChanges();

            var result = await _controller.DeleteFileRecord(record.Id);

            result.Should().BeOfType<NoContentResult>();
            _dbContext.FileRecords.Find(record.Id).Should().BeNull();
        }
    }
}
