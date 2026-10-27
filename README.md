# PetCare360 API

API RESTful desenvolvida em ASP.NET Core para monitoramento contínuo da saúde de pets, parte do Challenge 2026 — FIAP x Clyvo Vet.

## Descrição do Projeto

A PetCare360 API é a camada de backend do sistema de monitoramento inteligente de saúde animal da Clyvo Vet. A solução transforma a jornada de saúde do pet de um modelo reativo e episódico para uma experiência contínua, preventiva e integrada.

A API permite o cadastro de pets vinculados a coleiras inteligentes (IoT), recebe telemetria dos sensores, calcula o status de saúde do animal, gera alertas automáticos em situações críticas e fornece histórico de monitoramento para responsáveis e clínicas veterinárias.

Nesta versão, o projeto também conta com recursos de **observabilidade**, incluindo Health Checks, logging estruturado, métricas, tracing distribuído e identificação por correlação de requisições, além de uma estrutura de **testes unitários e de integração automatizados**.

O projeto utiliza dois bancos de dados: **Oracle Database**, para persistência relacional principal, e **MongoDB**, para armazenamento complementar de documentos de telemetria dos sensores.

## Tecnologias Utilizadas

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core 9
- Oracle Database (`oracle.fiap.com.br`)
- Oracle.EntityFrameworkCore
- Entity Framework Core InMemory
- MongoDB
- MongoDB.Driver
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

| Nome | RM |
|---|---|
| Leonardo Zerbinatti de Sales | RM562992 |
| Luis Guilherme Borges Silva | RM562992 |
| Rafael de Freitas Moraes | RM563210 |
| Rafael Pascotte Mercadante | RM564928 |

## Arquitetura de Dados

### Oracle Database

O Oracle é o banco de dados relacional principal da aplicação. É utilizado para persistir as entidades relacionais do sistema, como usuários, pets, dispositivos, dados de sensores e alertas, conforme a implementação dos repositórios e do contexto de dados.

### MongoDB

O MongoDB é utilizado como armazenamento complementar para documentos de telemetria dos sensores.

A configuração padrão utiliza:

- **Banco de dados:** `PetCare360`
- **Coleção:** `telemetry`
- **Endereço local:** `mongodb://localhost:27017`

A coleção pode ser criada automaticamente pelo MongoDB na primeira gravação de documentos.

A conexão com o MongoDB é configurada por meio das configurações da aplicação e pode ser substituída por uma variável de ambiente, permitindo a execução em diferentes máquinas.

## Pré-requisitos

Antes de executar o projeto, certifique-se de que os seguintes componentes estejam disponíveis:

- .NET SDK 9.0
- Oracle Database acessível, com usuário e senha válidos
- MongoDB Community Server instalado e em execução, ou uma instância MongoDB acessível pela rede
- Git, caso o projeto seja obtido por meio do repositório

Para verificar a instalação do .NET:

```powershell
dotnet --info