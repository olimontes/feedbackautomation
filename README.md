# FeedbackAutomation

Sistema desenvolvido em **.NET + C# + PostgreSQL** para:

* Importar planilhas Excel de clientes
* Armazenar feedbacks no banco
* Identificar clientes com notas baixas
* Gerar mensagens automáticas com IA
* Futuramente enviar mensagens por WhatsApp/SMS/E-mail

---

# Tecnologias utilizadas

* .NET 9 / C#
* Entity Framework Core
* PostgreSQL
* Docker
* ClosedXML (leitura de Excel)
* ASP.NET Core
* OpenAI API (futuramente)

---

# Estrutura do projeto

```bash
src/
├── AI/
├── Controllers/
├── Data/
├── DTOs/
├── Entities/
├── Imports/
├── Messaging/
├── Repositories/
├── Services/
└── Utils/
```

---

# Requisitos

Antes de rodar o projeto você precisa ter instalado:

## 1. .NET SDK

Baixe:

* [.NET SDK](https://dotnet.microsoft.com/download?utm_source=chatgpt.com)

Verifique:

```bash
dotnet --version
```

---

## 2. Docker Desktop

Baixe:

* [Docker Desktop](https://www.docker.com/products/docker-desktop/?utm_source=chatgpt.com)

Verifique:

```bash
docker --version
```

---

# Banco de Dados PostgreSQL

O projeto utiliza PostgreSQL via Docker.

## docker-compose.yml

```yml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine

    container_name: feedbackautomation-postgres

    environment:
      POSTGRES_DB: feedbackautomation
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres

    ports:
      - "5432:5432"

    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

---

# Como rodar o banco

Na raiz do projeto:

```bash
docker compose up -d
```

Verificar container:

```bash
docker ps
```

---

# Configuração do appsettings.json

Arquivo:

```bash
appsettings.json
```

Conteúdo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=feedbackautomation;Username=postgres;Password=postgres"
  }
}
```

---

# Instalar dependências

## Entity Framework

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

## Excel

```bash
dotnet add package ClosedXML
```

---

# Criar banco e tabelas

## Criar migration

```bash
dotnet ef migrations add InitialCreate
```

## Aplicar migration

```bash
dotnet ef database update
```

---

# Estrutura do banco

## Tabela Clientes

| Campo         | Tipo   |
| ------------- | ------ |
| Id            | int    |
| CodigoCliente | string |
| NomeRede      | string |
| Cnpj          | string |
| Responsavel   | string |
| Telefone      | string |

---

## Tabela Feedbacks

| Campo                    | Tipo     |
| ------------------------ | -------- |
| Id                       | int      |
| ClienteId                | int      |
| EquipeDescricao          | string   |
| NivelSatisfacao          | int      |
| DataVerificacaoQualidade | DateTime |
| MensagemGeradaIA         | string   |

---

# Formato da planilha Excel

A planilha precisa conter as colunas:

| Coluna                 |
| ---------------------- |
| codigo_cliente         |
| nomerede               |
| cnpj                   |
| responsavel            |
| telefone               |
| equidescricao_         |
| nivelsatisfacao        |
| dataverificaoqualidade |

---

# Importação do Excel

Classe responsável:

```bash
src/Imports/ExcelImporter.cs
```

Ela:

* Lê o Excel
* Verifica se cliente já existe pelo CNPJ
* Cria clientes
* Cria feedbacks
* Salva no PostgreSQL

---

# Como executar o projeto

Na raiz:

```bash
dotnet run
```

---

# Como verificar os dados no banco

Entrar no PostgreSQL:

```bash
docker exec -it feedbackautomation-postgres psql -U postgres -d feedbackautomation
```

Ver clientes:

```sql
SELECT * FROM "Clientes";
```

Ver feedbacks:

```sql
SELECT * FROM "Feedbacks";
```

---

# Fluxo atual do sistema

## O que já foi implementado

✅ PostgreSQL com Docker
✅ Entity Framework configurado
✅ Migrations funcionando
✅ Importação de Excel
✅ Persistência no banco
✅ Relacionamento Cliente ↔ Feedback
✅ Tratamento de duplicidade por CNPJ
✅ Estrutura inicial de IA
✅ Estrutura de mensageria

---

# Próximos passos

## 1. Integração com IA

Objetivo:

* Gerar mensagens automáticas para notas baixas

Exemplo:

> "Olá João, sentimos muito pela experiência ruim. Gostaríamos de entender melhor o ocorrido para melhorar nosso atendimento."

---

## 2. Envio de mensagens

Possibilidades:

* WhatsApp
* SMS
* E-mail

Sugestão:

* Twilio
* Evolution API
* Z-API
* Meta WhatsApp Cloud API

---

## 3. API REST

Endpoints futuros:

```http
GET /clientes
GET /feedbacks
POST /importar
POST /mensagem/enviar
```

---

# Possível arquitetura final

```text
Excel
   ↓
Importer
   ↓
Banco PostgreSQL
   ↓
Service
   ↓
IA
   ↓
Mensagem gerada
   ↓
WhatsApp / SMS / E-mail
```

---

# Observações importantes

## Datas no PostgreSQL

Foi necessário converter datas para UTC:

```csharp
DateTime.SpecifyKind(data, DateTimeKind.Utc)
```

Porque o PostgreSQL não aceita `timestamp with time zone` sem UTC.

---

# Comandos úteis

## Rodar projeto

```bash
dotnet run
```

## Criar migration

```bash
dotnet ef migrations add NomeMigration
```

## Atualizar banco

```bash
dotnet ef database update
```

## Ver containers

```bash
docker ps
```

## Derrubar containers

```bash
docker compose down
```

---

# Objetivo do projeto

Automatizar o processo de:

* leitura de feedbacks,
* identificação de clientes insatisfeitos,
* geração de mensagens inteligentes,
* recuperação de relacionamento com clientes.
