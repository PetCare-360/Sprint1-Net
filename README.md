# PetCare360 API

API RESTful desenvolvida em ASP.NET Core para monitoramento contínuo da saúde de pets, parte do Challenge 2026 — FIAP x Clyvo Vet.

## Descrição do Projeto

A PetCare360 API é a camada de backend do sistema de monitoramento inteligente de saúde animal da Clyvo Vet. A solução transforma a jornada de saúde do pet de um modelo reativo e episódico para uma experiência contínua, preventiva e integrada.

A API permite o cadastro de pets vinculados a coleiras inteligentes (IoT), recebe telemetria dos sensores, calcula o status de saúde do animal, gera alertas automáticos em situações críticas e fornece histórico de monitoramento para responsáveis e clínicas veterinárias.

Nesta versão, o projeto também conta com recursos de **observabilidade**, incluindo Health Checks, logging estruturado, métricas, tracing distribuído e identificação por correlação de requisições, além de uma estrutura de **testes unitários e de integração automatizados**.

## Tecnologias Utilizadas

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core 9
- Oracle Database (oracle.fiap.com.br)
- Oracle.EntityFrameworkCore
- Entity Framework Core InMemory
- JWT Bearer Authentication
- BCrypt.Net
- Swagger / OpenAPI (Swashbuckle)
- Serilog
- OpenTelemetry
- Health Checks
- xUnit
- Moq
- WebApplicationFactory
- Coverlet

## Integrantes

**Nome** e **RM**

Leonardo Zerbinatti de Sales - RM562992

Luis Guilherme Borges Silva - RM562992

Rafael de Freitas Moraes - RM563210

Rafael Pascotte Mercadante - RM564928

## Estrutura do Projeto

```text
PetCare360/
├── Controllers/
│   ├── AuthController.cs
│   ├── PetController.cs
│   ├── MonitoringController.cs
│   └── IotController.cs
├── Models/
│   ├── AppUser.cs
│   ├── Pet.cs
│   ├── Device.cs
│   ├── SensorData.cs
│   └── Alert.cs
├── Enums/
│   └── Enums.cs
├── DTOs/
│   ├── Requests/
│   │   └── Requests.cs
│   └── Responses/
│       └── Responses.cs
├── Data/
│   ├── AppDbContext.cs
│   └── AppDbContextFactory.cs
├── Repositories/
│   ├── Interfaces/
│   │   └── IRepositories.cs
│   └── Implementations/
│       └── Repositories.cs
├── Services/
│   ├── Interfaces/
│   │   └── IServices.cs
│   ├── AuthService.cs
│   ├── PetService.cs
│   ├── MonitoringService.cs
│   ├── IotProcessingService.cs
│   ├── CurrentUserService.cs
│   └── PetMapper.cs
├── Middleware/
│   ├── CorrelationIdMiddleware.cs
│   ├── GlobalExceptionMiddleware.cs
│   ├── ApplicationMetrics.cs
│   └── RequestMetricsMiddleware.cs
├── HealthChecks/
│   └── ExternalServiceHealthCheck.cs
├── Exceptions/
│   └── AppExceptions.cs
├── Migrations/
├── appsettings.json
├── Program.cs
└── PetCare360.csproj

PetCare360.Tests.Unit/
├── PetServiceTests.cs
└── PetServiceStatusTests.cs

PetCare360.Tests.Integration/
├── PetCareWebApplicationFactory.cs
├── AuthIntegrationTests.cs
└── PetIntegrationTests.cs
