# InvestScoreBackend

Backend em **.NET 10** para gestão de ativos financeiros, com APIs RESTful para Assets, AssetHeads e FileRecords.  
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
- [.NET 10](https://dotnet.microsoft.com/)
- [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)
- [OpenAI API](https://platform.openai.com/)

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
```
---
# TESTES UNITÁRIO E INTEGRAÇÃO


## 🧪 Testes Automatizados

O projeto possui uma suíte completa de **testes unitários e de integração**, garantindo a qualidade e confiabilidade das APIs.  
Abaixo estão descritos os principais testes e suas finalidades:

### 🔹 Testes Unitários

#### AssetsControllerTests
- **GetAssets_Should_Return_List_Of_Assets** → valida que a listagem de ativos retorna corretamente.
- **GetAsset_Should_Return_Asset_When_Found** → garante que um ativo específico é retornado quando encontrado.
- **GetAsset_Should_Return_NotFound_When_NotFound** → verifica que o retorno é `404 NotFound` quando o ativo não existe.
- **CreateAsset_Should_Return_Created_Asset** → assegura que a criação de um ativo retorna o objeto criado.
- **UpdateAsset_Should_Return_Updated_Asset** → valida que a atualização de um ativo modifica os dados corretamente.
- **UpdateAsset_Should_Return_NotFound_When_Asset_Does_Not_Exist** → garante que atualizar um ativo inexistente retorna `404 NotFound`.
- **DeleteAsset_Should_Return_NoContent_When_Success** → confirma que a exclusão de um ativo existente retorna `204 NoContent`.
- **DeleteAsset_Should_Return_NotFound_When_Asset_Does_Not_Exist** → valida que excluir um ativo inexistente retorna `404 NotFound`.

#### FileRecordsControllerTests
- **GetFileRecords_Should_Return_List** → valida que a listagem de registros de arquivos retorna corretamente.
- **GetFileRecord_Should_Return_Record_When_Found** → garante que um registro específico é retornado quando encontrado.
- **GetFileRecord_Should_Return_NotFound_When_NotFound** → verifica que o retorno é `404 NotFound` quando o registro não existe.
- **CreateFileRecord_Should_Return_Created_Record** → assegura que a criação de um registro retorna o objeto criado.
- **DeleteFileRecord_Should_Return_NoContent_When_Success** → confirma que a exclusão de um registro existente retorna `204 NoContent`.

#### PromptsControllerTests
- **ExecutePrompt_Should_Return_Assets_When_Success** → valida que a execução de um prompt retorna ativos corretamente.
- **ExecutePrompt_Should_Return_BadRequest_When_ServiceThrowsException** → garante que erros na execução do prompt retornam `400 BadRequest`.

---

### 🔹 Testes de Integração

#### AssetsControllerIntegrationTests
- **GetAssets_Should_Return_All_Assets** → valida a listagem de ativos persistidos no banco em memória.
- **GetAsset_Should_Return_Asset_When_Found** → garante que um ativo específico é retornado corretamente.
- **CreateAsset_Should_Persist_Asset** → assegura que a criação de um ativo persiste no banco.
- **UpdateAsset_Should_Modify_Existing_Asset** → valida que a atualização modifica os dados no banco.
- **DeleteAsset_Should_Remove_Asset** → confirma que a exclusão remove o ativo do banco.

#### FileRecordsControllerIntegrationTests
- **CreateFileRecord_Should_Persist_Record** → valida que a criação de registros de arquivos persiste no banco.
- **GetFileRecords_Should_Return_All_Records** → garante que a listagem retorna todos os registros.
- **GetFileRecord_Should_Return_Record_When_Found** → assegura que um registro específico é retornado.
- **DeleteFileRecord_Should_Remove_Record** → confirma que a exclusão remove o registro do banco.

#### FullFlowChainedIntegrationTests
- **FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets** → valida o fluxo completo: criar registro de arquivo → executar prompt (simulado com `HttpClientFactoryFake`) → consultar ativos persistidos.

#### FullFlowTestServerTests
- **FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets** → valida o fluxo completo usando `TestServer` com banco em memória.

#### FullFlowTestServerWithFakeTests
- **FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets** → valida o fluxo completo usando `TestServer` e `HttpClientFactoryFake` para simular respostas da OpenAI.

#### FullFlowTestServerParametrizedTests
- **FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets (parametrizado)** → executa o fluxo completo com diferentes tickers (`AAPL`, `MSFT`, `TSLA`) usando `[Theory]` e `InlineData`.

#### FullFlowWebAppFactoryTests
- **FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets** → valida o fluxo completo usando `WebApplicationFactory` com banco em memória.

#### FullFlowWebAppFactoryWithFakeTests
- **FullFlow_Should_CreateFileRecord_ExecutePrompt_And_QueryAssets** → valida o fluxo completo usando `WebApplicationFactory` e `HttpClientFactoryFake` para simular respostas da OpenAI.

---

## ✅ Benefícios dos Testes
- Garantem que os **CRUDs** funcionam corretamente.
- Validam fluxos completos de integração entre **FileRecords → Prompts → Assets**.
- Permitem simular chamadas externas à **OpenAI** sem depender da API real.
- Asseguram confiabilidade e evitam regressões durante evoluções do sistema.
