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
![Relatório de Doações Recentes](https://github.com/user-attachments/assets/556c407c-b2fe-42bb-bbd4-038cd24b3b1f)
![Relatório de Estoque de Sangue](https://github.com/user-attachments/assets/b11e9a9f-0ae0-434f-9c5f-1039e48f05e5)


## 🛠️ Tecnologias e Padrões

### Core Stack

- ASP.NET Core 8
- Entity Framework Core 8
- SQL Server
- Rabbit MQ
- Docker

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

### Instalação

1. Clone o repositório:

   ```bash
   git clone https://github.com/VitorEspinoza/BloodBank.git

   ```

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
  }
}
```

### Usando Docker Compose

Este projeto utiliza Docker Compose para facilitar a configuração dos serviços necessários (RabbitMQ e SQL Server). Todas as dependências são provisionadas automaticamente.

#### Serviços Configurados

- 🐇 **RabbitMQ 4** (mensageria)

  - Interface de administração: http://localhost:15672 (usuário: `guest`, senha: `guest`)
  - Porta AMQP: 5672

- 🗃️ **SQL Server 2022** (banco de dados)
  - Conexão: `localhost,1433`
  - Usuário: `sa`
  - Senha: `Senha@Forte123!`

##### ⚙️ Pré-requisitos

- 🐋 **Docker** (rodando em background)  
  [![Docker Status](https://img.shields.io/badge/Docker-Running-2496ED?logo=docker)](https://docs.docker.com/get-docker/)


#### Como usar

1. **Iniciar os serviços**:

   ```bash
   # Navegue até a pasta raiz do projeto (onde está o arquivo docker-compose.yml)
   cd caminho/para/projeto

   # Inicie os containers em modo interativo para ver os logs
   docker-compose up

   # OU inicie em modo background
   docker-compose up -d
   ```

2. **Verificar status dos serviços**:

   ```bash
   docker ps
   ```

3. **Parar os serviços**:

   ```bash
   docker-compose down
   ```

4. **Remover volumes e dados persistentes** (opcional):
   ```bash
   docker-compose down -v
   ```

#### Observações Importantes

- Se executar em modo background (`-d`), aguarde pelo menos 10-15 segundos para que o RabbitMQ inicialize completamente antes de iniciar a aplicação.
- Todos os dados são persistidos em volumes Docker, portanto sobreviverão ao reinício dos containers.
- Para problemas de conexão com o RabbitMQ, tente executar sem a flag `-d` para ver os logs em tempo real.


#### 🛠️ Configuração do Banco

Execute para aplicar as migrations:

```bash
dotnet ef database update --project BloodBank.Infrastructure
```

#### ▶️ Executando a API

**AVISO IMPORTANTE**: O comando abaixo só funcionará se você tiver configurado todos os pré requisitos da aba **Essenciais** e estiver com os containers rodando (Ver seção **Usando Docker Compose**).

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
