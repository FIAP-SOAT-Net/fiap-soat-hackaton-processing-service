# FIAP SOAT Hackathon - Processing Service

Serviço responsável por receber, persistir e atualizar o ciclo de processamento de arquivos no ecossistema do hackathon.

Ele expõe uma API HTTP para consulta e atualização dos arquivos em processamento e também integra com RabbitMQ, MySQL e o `Analyzer Service`.

## Visão geral

### Responsabilidades principais

- registrar arquivos recebidos para processamento;
- consultar arquivos por id e por filtros;
- atualizar o status de processamento;
- publicar eventos de domínio;
- enviar o arquivo para o `Analyzer Service`;
- expor health check e endpoints HTTP para consumo externo.

### Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- RabbitMQ
- Testes com `NUnit`
- Docker / Docker Compose

## Estrutura do projeto

```text
src/
  Fiap.Soat.Hackaton.ProcessingService.Api/
  Fiap.Soat.Hackaton.ProcessingService.Application/
  Fiap.Soat.Hackaton.ProcessingService.Domain/
  Fiap.Soat.Hackaton.ProcessingService.Infrastructure/

tests/
  Fiap.Soat.Hackaton.ProcessingService.Api.Tests/
  Fiap.Soat.Hackaton.ProcessingService.Application.Tests/
  Fiap.Soat.Hackaton.ProcessingService.Domain.Tests/
  Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests/
  Fiap.Soat.Hackaton.ProcessingService.Integration.Tests/
```

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/)
- Docker e Docker Compose
- MySQL 8.4
- RabbitMQ 3.13+

## Como executar localmente

### 1. Subir a stack de infraestrutura e serviços

O repositório possui um compose local com a stack necessária para desenvolvimento:

```zsh
docker compose -f docker-compose.processing.local.yml up -d --build
```

Esse arquivo sobe:

- MySQL
- Flyway
- MongoDB
- RabbitMQ
- LocalStack
- `processing-service-api`
- `analyzer-service-api`

> Observação: o `processing-service-api` usa MySQL e RabbitMQ; o `analyzer-service-api` é usado para o fluxo de análise dos arquivos.

### 2. Rodar a API sem Docker

Se preferir rodar direto pelo SDK:

```zsh
dotnet run --project src/Fiap.Soat.Hackaton.ProcessingService.Api/Fiap.Soat.Hackaton.ProcessingService.Api.csproj
```

Verifique antes as configurações em `src/Fiap.Soat.Hackaton.ProcessingService.Api/appsettings.Development.json`.

## Configuração de ambiente

As configurações principais ficam em `appsettings.json` / `appsettings.Development.json` e também podem ser sobrescritas por variáveis de ambiente.

### Variáveis mais importantes

| Variável | Descrição |
| --- | --- |
| `ConnectionStrings__MySql` | string de conexão com o MySQL |
| `RabbitMQ__Host` | host do RabbitMQ |
| `RabbitMQ__Port` | porta do RabbitMQ |
| `RabbitMQ__User` | usuário do RabbitMQ |
| `RabbitMQ__Password` | senha do RabbitMQ |
| `RabbitMQ__ExchangeName` | exchange principal de eventos |
| `RabbitMQ__QueueName` | fila principal de eventos |
| `RabbitMQ__RoutingKey` | routing key dos eventos |
| `AnalyzerService__BaseUrl` | base URL do serviço de análise |
| `ASPNETCORE_ENVIRONMENT` | ambiente da aplicação |
| `ASPNETCORE_URLS` | urls expostas pela aplicação |

## Endpoints

### Health

```http
GET /health
```

### Processamento de arquivos

```http
GET /processingFiles
GET /processingFiles/{fileId}
PATCH /processingFiles/{fileId}
```

#### Exemplos

Listar arquivos:

```http
GET /processingFiles?status=RECEIVED
```

Atualizar status:

```http
PATCH /processingFiles/{fileId}
Content-Type: application/json

{
  "status": "PROCESSING"
}
```

## Testes

O projeto possui testes unitários e de integração, organizados por camada.

### Executar todos os testes de um projeto específico

```zsh
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Api.Tests/Fiap.Soat.Hackaton.ProcessingService.Api.Tests.csproj --nologo
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Application.Tests/Fiap.Soat.Hackaton.ProcessingService.Application.Tests.csproj --nologo
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Domain.Tests/Fiap.Soat.Hackaton.ProcessingService.Domain.Tests.csproj --nologo
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests/Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests.csproj --nologo
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Integration.Tests/Fiap.Soat.Hackaton.ProcessingService.Integration.Tests.csproj --nologo
```

### Executar todos em sequência

```zsh
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Api.Tests/Fiap.Soat.Hackaton.ProcessingService.Api.Tests.csproj --nologo && \
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Application.Tests/Fiap.Soat.Hackaton.ProcessingService.Application.Tests.csproj --nologo && \
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Domain.Tests/Fiap.Soat.Hackaton.ProcessingService.Domain.Tests.csproj --nologo && \
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests/Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests.csproj --nologo && \
dotnet test tests/Fiap.Soat.Hackaton.ProcessingService.Integration.Tests/Fiap.Soat.Hackaton.ProcessingService.Integration.Tests.csproj --nologo
```

## Docker Compose

### Stack local completa

```zsh
docker compose -f docker-compose.processing.local.yml up -d --build
```

### Stack com imagens publicadas

Se as imagens já estiverem publicadas no Docker Hub, use:

```zsh
docker compose -f docker-compose.processing.hub.yml up -d
```

## Observações

- O projeto usa `NUnit` para testes.
- A API expõe health check e endpoints de processamento.
- A camada de infraestrutura integra com MySQL e RabbitMQ.
- O serviço também chama o `Analyzer Service` para enviar os arquivos processados.

