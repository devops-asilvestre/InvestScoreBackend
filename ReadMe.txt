# cd C:\Users\Cliente\source\repos\2026\InvestScoreBackend
# dotnet clean; dotnet restore; dotnet build
# dotnet run --project InvestScoreBackend.csproj

# dotnet ef migrations add InitialCreate -p src\InvestScoreBackend.Infrastructure -s src\InvestScoreBackend.API
# dotnet ef database update -p src\InvestScoreBackend.Infrastructure -s src\InvestScoreBackend.API
# dotnet ef migrations remove -p src\InvestScoreBackend.Infrastructure -s src\InvestScoreBackend.API

# Onde obter a chave
# Crie uma conta em OpenAI.(https://platform.openai.com/)
# Vá em API Keys no painel.
# Gere uma nova chave secreta (exemplo: sk-abc123...).
# Essa chave deve ser mantida segura e nunca exposta no código-fonte.
# Adicione a chave ao arquivo appsettings.Development.json na seção OpenAI:

############################################################
# Exemplo upload prompt

{
  "filePath": "C:\\Users\\Cliente\\source\\repos\\2026\\InvestScoreBackend\\src\\InvestScoreBackend.API\\Prompts\\todos_ativos_b3.txt",
  "title": "Todos os ativos da B3", 
  "description": "Prompt para buscar a lista completa de ativos listados na B3"
}

{
  "filePath": "C:\\Users\\Cliente\\source\\repos\\2026\\InvestScoreBackend\\src\\InvestScoreBackend.API\\Prompts\\ativos_especificos_b3.txt",
  "title": "Analise os seguintes ativos da B3 - PETR4.SA, VALE3.SA, ITUB4.SA, BBDC4.SA", 
  "description": "Analise os seguintes ativos da B3 - PETR4.SA, VALE3.SA, ITUB4.SA, BBDC4.SA"
}

{
  "filePath": "C:\\Users\\Cliente\\source\\repos\\2026\\InvestScoreBackend\\src\\InvestScoreBackend.API\\Prompts\\ativos_internacionais.txt",
  "title": "Todos os ativos internacionais", 
  "description": "Analise os seguintes tickers internacionais e busque seus indicadores financeiros: AAPL, MSFT, TSLA"
}