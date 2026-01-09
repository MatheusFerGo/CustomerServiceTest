# 🍔 ControlePedidos - Customer Context
[![codecov](https://codecov.io/gh/MatheusFerGo/ControlePedidos.Customer/graph/badge.svg?token=CODECOV_TOKEN)](https://codecov.io/gh/MatheusFerGo/CUstomerServiceTest)

Este microsserviço é responsável pela **gestão e identificação de clientes** dentro do ecossistema **ControlePedidos**.  
Ele foi construído pensando em **Clean Architecture**, **DDD** e **SOLID**, garantindo que a lógica de negócio seja **independente de infraestrutura**.

---

## 🚀 Como Executar o Projeto

### 🐳 Via Docker (Recomendado)

Para subir o ambiente completo (**API + Banco de Dados PostgreSQL**), utilize o **Docker Compose**:

1. Certifique-se de que o **Docker Desktop** está rodando.
2. Na raiz do projeto, execute:

```bash
docker-compose up -d --build
```

Acesse o Swagger em:  
👉 `http://localhost:5000/swagger` (ou a porta configurada)

---

## 💻 Localmente (.NET CLI)

Se preferir rodar apenas a aplicação localmente:

1. Ajuste a **ConnectionString** no arquivo `appsettings.Development.json` para apontar para o seu banco local.
2. Execute as migrações do banco de dados:

```bash
dotnet ef database update --project src/ControlePedidos.CustomerContext.Infrastructure
```

Inicie a aplicação:

```bash
dotnet run --project src/ControlePedidos.CustomerContext.Api
```

## 🧪 Testes e Qualidade

O projeto possui uma **meta de 80% de cobertura de código**.  
A suíte de testes utiliza **Testcontainers** para subir bancos de dados reais durante a execução.

Para rodar os testes e gerar o relatório de cobertura:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

> ⚠️ **Nota**  
> Para rodar os testes de integração, o **Docker deve estar ativo**, pois o **Testcontainers** gerencia automaticamente o banco de dados temporário.

---

## 🛠️ Tecnologias Principais

- **.NET 9** (Runtime)
- **Entity Framework Core** (ORM)
- **PostgreSQL** (Banco de Dados)
- **FluentValidation** (Regras de Domínio)
- **xUnit & Moq** (Testes)
