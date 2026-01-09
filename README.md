# InvestScoreBackend

Backend em **.NET 8** para gestão de ativos financeiros, com APIs RESTful para Assets, AssetHeads e FileRecords.  
O projeto utiliza **Entity Framework Core** para persistência e suporta importação de dados de arquivos, leitura de conteúdo e cálculo de métricas financeiras.  
Além disso, integra com **OpenAI** para processamento de prompts e enriquecimento de dados.

---

## 🚀 Funcionalidades
- CRUD completo para **Assets** (ativos financeiros).
- CRUD completo para **FileRecords** (arquivos processados).
- CRUD para **AssetHeads** (cabeçalhos de processamento).
- Importação de ativos via integração com **OpenAI** e serviços externos.
- Persistência com **Entity Framework Core** e SQL Server.
- Configuração de relacionamentos entre entidades sem duplicações de FKs.

---

## 🛠️ Tecnologias
- [.NET 8](https://dotnet.microsoft.com/)
- [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- SQL Server
- [OpenAI API](https://platform.openai.com/)

---


---

## ⚙️ Configuração

### 1. Banco de dados
Configure a connection string no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InvestScoreBackendDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

