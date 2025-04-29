# 🩸 Sistema de Gerenciamento de Banco de Sangue

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-brightgreen)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

Sistema completo para gerenciamento de doadores, doações e estoque de sangue, implementando boas práticas de desenvolvimento e arquitetura limpa.

## 📌 Features Principais

### 🧑‍🤝‍🧑 Gestão de Doadores
- Cadastro com validação completa de elegibilidade
- Histórico de doações individual
- Restrições por período (90 dias mulheres/60 dias homens)

### 🏦 Controle de Estoque
- Atualização automática ao registrar doações
- Rastreamento por tipo sanguíneo e fator RH

### ✉️ Comunicação
- **Brevo (Sendinblue)** para envio de:
  - Agradecimentos pós-doção
  - Certificado de doação
- Notificações assíncronas via **RabbitMQ**

### 📄 Documentação
- PDFS Gerados com **QuestPDF**
    - Geração de **certificados de doação**;
    - **Relatórios** periódicos:
        - Quantidade total de sangue por tipo disponível
        - Doações nos últimos 30 dias com informações dos doadores
     
## Confira abaixo os relatórios e certificado: 

![Certificado de Doação](https://github.com/user-attachments/assets/fc56ff6f-762d-46cb-9567-9677d4a13503)
![Relatório de Estoque de Sangue](https://github.com/user-attachments/assets/65712c67-8125-40f3-8b9c-2a95e5c4857e)
![Relatório de Doações Recentes](https://github.com/user-attachments/assets/d992e1dc-dad4-4c0b-9703-a2deda5e7cb0)



## 🛠️ Tecnologias e Padrões

### Core Stack
- ASP.NET Core 8
- Entity Framework Core 8
- SQL Server

### Arquitetura
- Clean Architecture
- CQRS com MediatR
- Domain-Driven Design (DDD)
- Unit of Work
- Repository Pattern

### Integrações
- **Brevo API** (comunicação por email)
- **ViaCEP** (validação de endereços)

### Mensageria com RabbitMQ
- **Configuração**:
  - Channel Pooling
  - Padrão Outbox
    - Circuit Breaker & Retry (Polly)
  - Idempotência
- **Resiliência**:
  - `bloodbank.dlx` (Dead Letter Exchange)
  - `bloodbank.healthcheck` (Health Check Exchange)

### Qualidade
- Testes Unitários
    - **xUnit** como framework base  
    - **NSubstitute** para criação de mocks
    - **Bogus** para criação de dados falsos
- Testes Integração
    - TestContainers com **WireMock**, **RabbitMQ** e **SQLServer**
    

## ⚙️ Configuração

#### ⚠️ Aviso de Segurança
As credenciais abaixo são apenas para desenvolvimento local. Nunca as utilize em produção.

#### Essenciais
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (v8.0.x)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (v4.25+)
- **Serviços via Docker**:
  - 🐇 [RabbitMQ 4](https://www.rabbitmq.com/) (mensageria)
    ```bash
     docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
    ```
  - 🗃️ [SQL Server 2022](https://www.microsoft.com/sql-server) (banco de dados)
    ```bash
    docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Senha@Forte123!" -p 1433:1433 --name sql-server -d mcr.microsoft.com/mssql/server:2022-latest
    ```

#### Dependências via Container (**Próxima etapa**)
 **Ainda não implementado**: Em breve haverá docker compose para provisionamento automático do RabbitMQ e SQL Server.

### Instalação
1. Clone o repositório:
   ```bash
   git clone https://github.com/VitorEspinoza/BloodBank.git

2. Configure a chave do brevo e a connection string no .NET Secrets:

```json
{
// Ajuste conforme sua configuração do brevo

 "Brevo": {
   "ApiKey": "sua-chave",
   "FromEmail": "seu-email@email.com",
   "FromName": "Sistema Banco de Sangue"
 },

// Ajuste conforme sua conexão

 "ConnectionStrings": {
    "BloodBankCs": "Server=localhost,1433;Database=BloodBank;User Id=sa;Password=Senha@Forte123!;TrustServerCertificate=True;Encrypt=False;"
  },
}
```

#### 🛠️ Configuração do Banco
Execute para aplicar as migrations:
 ```bash
 dotnet ef database update --project BloodBank.Infrastructure
```

#### ▶️ Executando a API
 **AVISO IMPORTANTE**: O comando abaixo só funcionará se você tiver configurado todos os pré requisitos da aba **Essenciais**. 

Na raiz do projeto:
 ```bash
dotnet run --project BloodBank.API
```

## 🧪 Testes

#### 🔬 Testes Unitários
```bash
dotnet test BloodBank.Testing.UnitTests
```

#### 🧩 Testes de Integração

##### ⚙️ Pré-requisitos

- 🐋 **Docker** (rodando em background)  
  [![Docker Status](https://img.shields.io/badge/Docker-Running-2496ED?logo=docker)](https://docs.docker.com/get-docker/)
  
```bash
dotnet test BloodBank.Testing.IntegrationTests
```
