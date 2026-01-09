using InvestScoreBackend.Application.Services;
using InvestScoreBackend.Contracts.Repositories;
using InvestScoreBackend.Contracts.Services;
using InvestScoreBackend.Domain.Interfaces.Services;
using InvestScoreBackend.Infrastructure.Config;
using InvestScoreBackend.Infrastructure.Persistence;
using InvestScoreBackend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var alphaVantageApiKey = builder.Configuration["AlphaVantage:ApiKey"];

builder.Services.AddDbContext<InvestScoreDbContext>(options =>
    options.UseSqlServer(connectionString));

// Serviços de domínio / aplicação
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAssetHeadService, AssetHeadService>();
builder.Services.AddScoped<IFileRecordService, FileRecordService>();
builder.Services.AddScoped<IPromptService, PromptService>();

// Repositórios
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssetHeadRepository, AssetHeadRepository>();
builder.Services.AddScoped<IFileRecordRepository, FileRecordRepository>();


// HttpClient + configuração da API Key
builder.Services.AddHttpClient();
builder.Services.AddSingleton(new AlphaVantageConfig { ApiKey = alphaVantageApiKey });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
