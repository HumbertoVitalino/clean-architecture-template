# Clean Architecture Template — Contexto do Projeto

## Visão Geral

Template .NET para inicialização de projetos seguindo Clean Architecture com DDD.
Distribuído via `dotnet new`, permitindo que o usuário informe o nome da solução na criação.

```bash
dotnet new install CleanArchitecture.Template
dotnet new clean-arch -n MinhaEmpresa.MeuProduto
```

---

## Objetivo

Ser um ponto de partida opinativo e funcional — não apenas estrutura de pastas vazia, mas
código real de exemplo que demonstra como implementar cada conceito dentro da arquitetura.

---

## Stack & Versão Alvo

| Item              | Escolha                                                      |
|-------------------|--------------------------------------------------------------|
| Runtime           | .NET 10                                                      |
| Linguagem         | C# 13                                                        |
| API               | ASP.NET Core — Minimal API (padrão) ou Controllers (flag)   |
| ORM               | Entity Framework Core 10                                     |
| Banco (dev/test)  | SQL Server (via Docker)                                      |
| Auth              | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| Testes unitários  | xUnit + FluentAssertions + NSubstitute + Moq                 |
| Testes integrados | xUnit + Testcontainers.MsSql                                 |
| Testes funcionais | xUnit + WebApplicationFactory + HttpClient                   |

---

## Decisões Fechadas

| Decisão          | Escolha                                                                      |
|------------------|------------------------------------------------------------------------------|
| API Style        | **Minimal API** (padrão) — Controllers via `--use-controllers`               |
| Mediator         | **Sem MediatR** (padrão) — MediatR via `--use-mediatr`                       |
| Banco            | **SQL Server** (Docker)                                                       |
| Autenticação     | **JWT** incluído no template base                                             |
| UoW              | `IUnitOfWork.CommitAsync()` exposto via `IRepository.UnitOfWork`             |
| Persistence      | Model de persistência separado do domain entity + mapper `ToDomain/ToModel`  |
| DI               | Cada camada tem `IoC/DependencyInjection.cs` com método `AddX()`             |
| Testes unitários | DisplayName: `Method >> Should X >> When Y`; padrão AAA com comentários      |

---

## Estrutura de Camadas

```
src/
  CompanyName.ProjectName.Domain/
    Abstractions/          # AggregateRoot<TId>, IAggregateRoot, ValueObject, IDomainEvent, DomainException
    Users/                 # User, Email (VO), UserErrors, UserCreatedEvent

  CompanyName.ProjectName.Application/
    Abstractions/          # IRepository<T,TId>, IDomainEventHandler<T>
    Commons/               # Output (wrapper de resposta de use case)
    DTOs/Users/            # UserResponse
    Interfaces/
      Repositories/        # IUserRepository
      Services/            # ICurrentUserService, IDomainEventDispatcher
      UseCases/            # ICreateUserUseCase, IGetUserByIdUseCase
    UseCases/Users/
      CreateUser/          # UseCase + Boundaries (Input + Validator) + Mapper
      GetUserById/         # UseCase + Boundaries (Input) + Mapper
    IoC/                   # DependencyInjection.AddApplication()

  CompanyName.ProjectName.Infrastructure/
    Persistence/
      Models/              # UserModel (entidade EF Core)
      Configurations/      # UserModelConfiguration (IEntityTypeConfiguration)
      Mappers/             # UserMapper: ToDomain() / ToModel()
      Repositories/        # UserRepository (implements IUserRepository + IUnitOfWork via context)
      Migrations/          # InitialCreate
      AppDbContext.cs      # implements IUnitOfWork, despacha domain events pós-commit
      AppDbContextFactory  # design-time factory lê appsettings.json para migrations
    Services/
      CurrentUserService   # ICurrentUserService via IHttpContextAccessor + JWT claims
      DomainEventDispatcher# IDomainEventDispatcher via IServiceProvider
    IoC/                   # DependencyInjection.AddInfrastructure(IConfiguration)
    appsettings.json       # design-time only (connection string para migrations)

  CompanyName.ProjectName.Api/          # (feature/8) Endpoints, JWT, OpenAPI, Program.cs

tests/
  CompanyName.ProjectName.UnitTests/    # Domain + Application — sem I/O
  CompanyName.ProjectName.IntegrationTests/  # (feature/9) UserRepository contra SQL Server real (Testcontainers)
  CompanyName.ProjectName.FunctionalTests/   # (feature/10) Endpoints HTTP via WebApplicationFactory
```

---

## Conceitos DDD a Demonstrar

- **Aggregate Root** — entidade raiz com controle de invariantes e publicação de domain events
- **Value Object** — imutável, igualdade por valor (Email)
- **Domain Event** — publicado pelo aggregate, despachado na Infrastructure pós-CommitAsync
- **Repository** — interface em Application, implementação em Infrastructure com modelo de persistência separado
- **Use Case** — orquestra domínio; chama `repository.UnitOfWork.CommitAsync()` ao final
- **Unit of Work** — `CommitAsync() → bool` exposto via `IRepository.UnitOfWork`; implementado pelo `AppDbContext`

---

## Testes — Padrão AAA

```csharp
[Fact(DisplayName = "Create >> Should Return User >> When Input Is Valid")]
public void Create_ValidData_ReturnsUser()
{
    // Arrange
    const string name = "John Doe";

    // Act
    var user = User.Create("john@example.com", name);

    // Assert
    user.Name.Should().Be(name);
}
```

---

## Parâmetros do Template (`dotnet new`)

```bash
dotnet new clean-arch -n MinhaEmpresa.MeuProduto                              # padrão (Minimal API)
dotnet new clean-arch -n MinhaEmpresa.MeuProduto --use-controllers            # com controllers
dotnet new clean-arch -n MinhaEmpresa.MeuProduto --use-mediatr                # com MediatR
dotnet new clean-arch -n MinhaEmpresa.MeuProduto --use-controllers --use-mediatr
```

---

## Estratégia de Branches

```
release     ← branch de produção (merge sempre via PR automático vindo de develop)
develop     ← branch de integração (toda feature vai para cá)
feature/*   ← uma branch por task do plano
fix/*       ← correções pontuais
```

---

## GitHub Actions

| Arquivo              | Trigger               | Jobs                                                                |
|----------------------|-----------------------|---------------------------------------------------------------------|
| `ci-feature.yml`     | push `feature/**`     | Build → Unit Tests → Integration Tests → Functional Tests           |
| `ci-develop.yml`     | push `develop`        | Build → Unit Tests → Integration Tests → Functional Tests → PR auto |
| `codeql.yml`         | PR→`release` + weekly | Análise de segurança estática (CodeQL)                              |
| `cd.yml`             | push `release`        | Build Docker image + push GHCR + publish NuGet template             |

Cada job de teste sobe artefato `.trx` independente. O PR automático para `release` só
dispara quando `functional-tests` passa.

---

## Plano de Desenvolvimento

Ver [PLAN.md](PLAN.md) — features organizadas em 4 ciclos de develop com critérios de saída.

---

## Estado Atual

### Concluído
- [x] **Develop 1** — Fundação (features 1–3): estrutura de solução, GitHub Actions, template config
- [x] **Develop 2** — Núcleo de Domínio (features 4–6): Domain layer, Application layer, Unit Tests
- [x] **feature/7** — Infrastructure layer (aguardando merge na develop)
- [x] **fix/ci-feature-steps** — CI split em jobs separados + fix Moq ambiguity (aguardando merge)

### Em andamento — Develop 3
- [ ] **feature/8** — API layer (Minimal API, JWT, OpenAPI, ProblemDetails)
- [ ] **feature/9** — Integration Tests (Testcontainers.MsSql, UserRepository tests)
- [ ] **feature/10** — Functional Tests (WebApplicationFactory, endpoint tests)

### Pendente — Develop 4
- [ ] **feature/11** — Docker (Dockerfile multi-stage, docker-compose.yml com SQL Server + volume)
- [ ] **feature/12** — NuGet publish (dotnet pack, README, badges)
