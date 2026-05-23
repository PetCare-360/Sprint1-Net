# PetCare360 API

API RESTful desenvolvida em ASP.NET Core para monitoramento contínuo da saúde de pets, parte do Challenge 2026 — FIAP x Clyvo Vet.

## 📋 Descrição do Projeto

A PetCare360 API é a camada de backend do sistema de monitoramento inteligente de saúde animal da Clyvo Vet. A solução transforma a jornada de saúde do pet de um modelo reativo e episódico para uma experiência contínua, preventiva e integrada.

A API permite o cadastro de pets vinculados a coleiras inteligentes (IoT), recebe telemetria em tempo real dos sensores, calcula o status de saúde do animal, gera alertas automáticos em situações críticas e fornece histórico completo de monitoramento para responsáveis e clínicas veterinárias.

## 🛠️ Tecnologias Utilizadas

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core 9
- Oracle Database (oracle.fiap.com.br)
- Oracle.EntityFrameworkCore
- JWT Bearer Authentication
- BCrypt.Net
- Swagger / OpenAPI (Swashbuckle)

## 👥 Integrantes

| Nome | RM |
|------|----|
| Arthur Correia Delila | RM563806 |
| Gabriel Henrique Souza Goncalves | RM563732 |
| Jose Ricardo Pereira Iannuzzi | RM564112 |
| Rafael de Freitas Moraes | RM563210 |
| Rafael Pascotte Mercadante | RM564928 |

## 🗂️ Estrutura do Projeto

```
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
│   └── GlobalExceptionMiddleware.cs
├── Exceptions/
│   └── AppExceptions.cs
├── Migrations/
├── appsettings.json
├── Program.cs
└── PetCare360.csproj
```

## ⚙️ Como Instalar e Executar

### Pré-requisitos

- .NET 9 SDK instalado — https://dotnet.microsoft.com/download
- Acesso à rede da FIAP (presencial ou VPN) para conexão com o Oracle
- Git instalado

### Passo a passo

**1. Clonar o repositório**
```bash
git clone https://github.com/seu-usuario/seu-repositorio.git
cd seu-repositorio/PetCare360
```

**2. Configurar o banco de dados**

Abra o arquivo `appsettings.json` e preencha com suas credenciais Oracle da FIAP:
```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=SEU_RM;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL;"
  },
  "Jwt": {
    "Secret": "PetCare360SuperSecretKeyMinimo32Caracteres!",
    "Issuer": "PetCare360.Api",
    "Audience": "PetCare360.Client"
  }
}
```

**3. Restaurar os pacotes**
```bash
dotnet restore
```

**4. Instalar a ferramenta do EF Core**
```bash
dotnet tool install --global dotnet-ef
```

**5. Criar as tabelas no banco Oracle**
```bash
dotnet ef database update
```

**6. Executar o projeto**
```bash
dotnet run
```

**7. Acessar o Swagger**

Abra o navegador em:
```
http://localhost:5000
```

## 🔒 Autenticação

A API utiliza autenticação JWT. Para acessar os endpoints protegidos:

1. Crie um usuário em `POST /auth/register`
2. Faça login em `POST /auth/login` e copie o `token` retornado
3. No Swagger, clique em **Authorize** e insira `Bearer SEU_TOKEN`

## 🚀 Endpoints

### Auth — `/auth`

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/auth/register` | Cadastrar novo usuário | ❌ |
| POST | `/auth/login` | Login e obtenção do token JWT | ❌ |

### Pets — `/pets`

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | `/pets` | Listar pets do usuário com paginação | ✅ |
| GET | `/pets/all` | Listar todos os pets sem paginação | ✅ |
| POST | `/pets` | Cadastrar pet com primeira leitura de sensores | ✅ |
| GET | `/pets/{id}` | Buscar pet por ID | ✅ |
| PUT | `/pets/{id}` | Atualizar dados do pet | ✅ |
| DELETE | `/pets/{id}` | Remover pet | ✅ |
| GET | `/pets/{id}/health-status` | Status consolidado de saúde do pet | ✅ |
| GET | `/pets/{id}/activity-summary` | Resumo de atividade das últimas 24h | ✅ |
| GET | `/pets/quick-alerts` | Pets em estado de alerta ou crítico | ✅ |

### Monitoramento — `/pets/{id}`

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | `/pets/{id}/summary` | Resumo atual com última leitura e totais | ✅ |
| GET | `/pets/{id}/monitoring` | Histórico de monitoramento paginado | ✅ |
| GET | `/pets/{id}/activity` | Histórico de atividade paginado | ✅ |
| GET | `/pets/{id}/location` | Última localização do pet | ✅ |
| GET | `/pets/{id}/alerts` | Alertas gerados para o pet | ✅ |

### IoT — `/api/iot`

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/iot/data` | Receber telemetria da coleira inteligente | ❌ |

## 📦 Exemplos de Requisição

### Cadastrar usuário
```json
POST /auth/register
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "123456"
}
```

### Cadastrar pet
```json
POST /pets
{
  "name": "Rex",
  "age": 3,
  "weight": 10.5,
  "breed": "Labrador",
  "deviceId": "DEVICE-001",
  "initialSensorData": {
    "timestamp": "2026-05-23T10:00:00Z",
    "temperature": 38.5,
    "heartRate": 80,
    "activityLevel": 60,
    "latitude": -23.5505,
    "longitude": -46.6333,
    "battery": 85
  }
}
```

### Enviar telemetria IoT
```json
POST /api/iot/data
{
  "deviceId": "DEVICE-001",
  "timestamp": "2026-05-23T10:05:00Z",
  "temperature": 41.5,
  "heartRate": 165,
  "activityLevel": 5,
  "latitude": -23.5505,
  "longitude": -46.6333,
  "battery": 15
}
```

## 📊 Códigos de Retorno HTTP

| Código | Descrição |
|--------|-----------|
| 200 | OK — requisição bem-sucedida |
| 201 | Created — recurso criado com sucesso |
| 204 | No Content — recurso removido com sucesso |
| 400 | Bad Request — dados inválidos |
| 401 | Unauthorized — token ausente ou inválido |
| 404 | Not Found — recurso não encontrado |
| 409 | Conflict — recurso já existente |
| 500 | Internal Server Error — erro interno |
