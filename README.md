[![Português](https://img.shields.io/badge/Português-🇧🇷_Leia_em_Português-green)](./README.pt-BR.md)

# 🩸 Blood Bank Management System

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-brightgreen)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

A complete system for managing blood donors, donations, and stock, implementing development best practices and Clean Architecture principles.

## 📌 Key Features

### 🧑‍🤝‍🧑 Donor Management

- Registration with full eligibility validation
- Individual donation history
- Gender-based donation restrictions (90 days for women/60 days for men)

### 🏦 Stock Control

- Automatic stock updates when registering donations
- Tracking by blood type and RH factor

### ✉️ Communication

- **Brevo (Sendinblue)** integration for:
  - Post-donation thank-you emails
  - Donation certificates
- Asynchronous notifications via **RabbitMQ**

### 📄 Documentation

- PDF generation with **QuestPDF**:
  - Donation certificates
  - Periodic reports:
    - Total blood stock by type
    - Recent donations (last 30 days) with donor information

## See sample reports and certificate below:

![Donation Certificate](https://github.com/user-attachments/assets/fc56ff6f-762d-46cb-9567-9677d4a13503)
![Recent Donations Report](https://github.com/user-attachments/assets/556c407c-b2fe-42bb-bbd4-038cd24b3b1f)
![Blood Stock Report](https://github.com/user-attachments/assets/b11e9a9f-0ae0-434f-9c5f-1039e48f05e5)

## 🛠️ Technologies and Patterns

### Core Stack

- ASP.NET Core 8
- Entity Framework Core 8
- SQL Server
- RabbitMQ
- Docker

### Architecture

- Clean Architecture
- CQRS with MediatR
- Domain-Driven Design (DDD)
- Unit of Work
- Repository Pattern

### Integrations

- **Brevo API** (email communication)
- **ViaCEP** (Brazilian postal code validation)

### RabbitMQ Messaging

- **Configuration**:
  - Channel Pooling
  - Outbox Pattern
    - Circuit Breaker & Retry (Polly)
  - Idempotency
- **Resilience**:
  - `bloodbank.dlx` (Dead Letter Exchange)
  - `bloodbank.healthcheck` (Health Check Exchange)

### Quality Assurance

- Unit Tests
  - **xUnit** as base framework
  - **NSubstitute** for mocking
  - **Bogus** for fake data generation
- Integration Tests
  - TestContainers with **WireMock**, **RabbitMQ** and **SQLServer**

## ⚙️ Setup

#### ⚠️ Security Notice

The credentials below are for local development only. Never use them in production.

#### Essentials

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (v8.0.x)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (v4.25+)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/VitorEspinoza/BloodBank.git
   ```
2. Configure the target key and connection string in .NET Secrets:

```json
{
  // Adjust according to your Brevo configuration

  "Brevo": {
    "ApiKey": "your-key",
    "FromEmail": "your-email@email.com",
    "FromName": "Blood Bank System"
  },

  // Adjust according to your connection

  "ConnectionStrings": {
    "BloodBankCs": "Server=localhost,1433;Database=BloodBank;User Id=sa;Password=Strong@Password123!;TrustServerCertificate=True;Encrypt=False;"
  }
}
```

### Using Docker Compose

This project uses Docker Compose to facilitate the configuration of the necessary services (RabbitMQ and SQL Server). All dependencies are provisioned automatically.

#### Configured Services

- **RabbitMQ 4** (messaging)

  - Administration interface: http://localhost:15672 (user: `guest`, password: `guest`)
  - AMQP port: 5672

- 🗃️ **SQL Server 2022** (database)
  - Connection: `localhost,1433`
  - User: `sa`
  - Password: `Strong@Password123!`

##### ⚙️ Prerequisites

- 🐋 **Docker** (running in the background)
  [![Docker Status](https://img.shields.io/badge/Docker-Running-2496ED?logo=docker)](https://docs.docker.com/get-docker/)

#### How to use

1. **Start the services**:

   ```bash
   # Navigate to the root folder of the project (where the docker-compose.yml file is)
   cd path/to/project

   # Start the containers in interactive mode to see the logs
   docker-compose up

   # OR start in background mode
   docker-compose up -d
   ```

2. **Check status of services**:

   ```bash
   docker ps
   ```

3. **Stop services**:

```bash
docker-compose down
```

4. **Remove persistent volumes and data** (optional):

```bash
docker-compose down -v
```

#### Important Notes

- If running in background mode (`-d`), wait at least 10-15 seconds for RabbitMQ to fully initialize before starting the application.
- All data is persisted on Docker volumes, so it will survive the restart of the containers.
- For problems connecting to RabbitMQ, try running it without the `-d` flag to see the logs in real time.

#### 🛠️ Database configuration

Run to apply the migrations:

```bash
dotnet ef database update --project BloodBank.Infrastructure
```

#### ▶️ Running the API

**IMPORTANT NOTICE: The command below will only work if you have configured all the pre-requisites of the **Essentials** tab and have the containers running (See section **Using Docker Compose\*\*).

In the root of the project:

```bash
dotnet run --project BloodBank.API
```

## 🧪 Testing

#### 🔬 Unit tests

```bash
dotnet test BloodBank.Testing.UnitTests
```

#### 🧩 Integration Tests

##### ⚙️ Prerequisites

- 🐋 **Docker** (running in the background)
  [![Docker Status](https://img.shields.io/badge/Docker-Running-2496ED?logo=docker)](https://docs.docker.com/get-docker/)
