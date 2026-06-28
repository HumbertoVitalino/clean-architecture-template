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
| Testes unitários  | xUnit + FluentAssertions + NSubstitute                       |
| Testes integrados | xUnit + Testcontainers (SQL Server)                          |
| Testes funcionais | xUnit + WebApplicationFactory + HttpClient                   |

---

## Decisões Fechadas

| Decisão        | Escolha                                                                      |
|----------------|------------------------------------------------------------------------------|
| API Style      | **Minimal API** (padrão) — Controllers via `--use-controllers`               |
| Mediator       | **Sem MediatR** (padrão) — MediatR via `--use-mediatr`                       |
| Banco          | **SQL Server** (Docker)                                                       |
| Autenticação   | **JWT** incluído no template base                                             |

---

## Estrutura de Camadas

```
src/
  CompanyName.ProjectName.Domain/           # Entidades, Value Objects, Domain Events, interfaces de repositório
  CompanyName.ProjectName.Application/      # Use Cases, DTOs, interfaces de serviço
  CompanyName.ProjectName.Infrastructure/   # EF Core, repositórios, migrations, serviços externos
  CompanyName.ProjectName.Api/              # Endpoints / Controllers, DI root, middleware, auth

tests/
  CompanyName.ProjectName.UnitTests/        # Testa Domain e Application isoladamente (sem I/O)
  CompanyName.ProjectName.IntegrationTests/ # Testa Infrastructure com SQL Server via Testcontainers
  CompanyName.ProjectName.FunctionalTests/  # Testa endpoints HTTP via WebApplicationFactory
```

---

## Conceitos DDD a Demonstrar

- **Aggregate Root** — entidade raiz com controle de invariantes e publicação de domain events
- **Value Object** — imutável, igualdade por valor
- **Domain Event** — publicado pelo aggregate, despachado na Infrastructure pós-SaveChanges
- **Repository (interface no Domain, impl na Infrastructure)** — sem vazamento de EF para o domínio
- **Use Case** — orquestra domínio sem lógica de negócio própria
- **Result Pattern** — `Result<T>` / `Error` para fluxo explícito sem exceções de controle

---

## Testes — Padrão AAA

Todos os testes unitários devem seguir o padrão AAA com comentários delimitadores.

```csharp
[Fact]
public void Should_CalculateTotal_When_ItemsAreAdded()
{
    // Arrange
    var order = new Order();
    order.AddItem(new OrderItem("Produto A", 2, 50.00m));

    // Act
    var total = order.Total;

    // Assert
    total.Should().Be(100.00m);
}
```

---

## Parâmetros do Template (`dotnet new`)

```bash
dotnet new clean-arch -n MinhaEmpresa.MeuProduto                              # padrão
dotnet new clean-arch -n MinhaEmpresa.MeuProduto --use-controllers            # com controllers
dotnet new clean-arch -n MinhaEmpresa.MeuProduto --use-mediatr                # com MediatR
dotnet new clean-arch -n MinhaEmpresa.MeuProduto --use-controllers --use-mediatr
```

Arquivos exclusivos de cada modo ficam em pastas separadas com `source/modifiers` no
`template.json`. Blocos dentro de arquivos `.cs` e `.csproj` usam diretivas `#if (useControllers)`.

---

## Estratégia de Branches

```
release     ← branch de produção (merge sempre via PR automático vindo de develop)
develop     ← branch de integração (toda feature vai para cá)
feature/*   ← uma branch por task do plano abaixo
```

### Fluxo

```
feature/1-solution-structure
  └─ PR → develop   (revisão + CI passa)
feature/2-template-config
  └─ PR → develop
...
develop
  └─ CI completa passa
      └─ Action verifica se branch `release` existe
          └─ Abre PR automático: develop → release
```

> Sem GMUD. O gate de qualidade é a pipeline — se passar, o PR é criado automaticamente.
> Aprovação e merge do PR para `release` ficam com o time.

### Convenção de nomes de branch

```
feature/<número>-<slug-curto>
ex: feature/3-domain-layer
    feature/6-api-layer
```

---

## GitHub Actions — Alinhados ao Fluxo de Branches

| Arquivo                    | Trigger                          | O que executa                                                       |
|----------------------------|----------------------------------|---------------------------------------------------------------------|
| `ci-feature.yml`           | push em `feature/*`              | build + testes unitários (feedback rápido)                          |
| `ci-develop.yml`           | push em `develop`                | suite completa → se OK e `release` existe → abre PR automático      |
| `codeql.yml`               | PR → `release` + schedule semanal| análise de segurança estática (CodeQL)                              |
| `cd.yml`                   | merge (push) em `release`        | build Docker image, push registry, publish NuGet template           |

### Detalhamento dos workflows

**`ci-feature.yml`** — feedback rápido, sem containers
```
on:
  push:
    branches: ['feature/**']

jobs:
  build-and-unit-test:
    - dotnet restore
    - dotnet build --no-restore
    - dotnet test --filter Category=Unit --no-build
```

**`ci-develop.yml`** — suite completa + abertura automática de PR
```
on:
  push:
    branches: ['develop']

jobs:
  build-and-test:
    - dotnet restore
    - dotnet build
    - dotnet test (UnitTests + IntegrationTests + FunctionalTests)

  open-release-pr:
    needs: build-and-test          # só executa se os testes passaram
    steps:
      - name: Verifica se branch release existe
        id: check-release
        run: |
          git ls-remote --exit-code --heads origin release
          echo "exists=true" >> $GITHUB_OUTPUT
        continue-on-error: true    # não falha a pipeline se a branch não existir

      - name: Abre PR develop → release
        if: steps.check-release.outputs.exists == 'true'
        uses: peter-evans/create-pull-request@v6   # ou gh pr create via CLI
        with:
          base: release
          head: develop
          title: "release: promote develop to release"
          body: "Pipeline passou. PR aberto automaticamente."
          labels: release
```

**`codeql.yml`** — segurança
```
on:
  pull_request:
    branches: ['release']
  schedule:
    - cron: '0 8 * * 1'   # toda segunda-feira

jobs:
  analyze:
    language: csharp
```

**`cd.yml`** — entrega contínua
```
on:
  push:
    branches: ['release']

jobs:
  docker:
    - docker build + push (GHCR)
  nuget:
    - dotnet pack
    - dotnet nuget push
```

---

## Plano de Desenvolvimento

Ver [PLAN.md](PLAN.md) — features organizadas em 4 ciclos de develop com critérios de saída.

## Estado Atual

- [x] Estrutura inicial com 4 projetos (Api, Application, Domain, Infrastructure)
- [x] Solução `.slnx`
- [x] `.gitignore` configurado
- [x] Decisões de arquitetura fechadas
- [x] Plano de desenvolvimento definido (`PLAN.md`)
- [ ] Develop 1 — Fundação
- [ ] Develop 2 — Núcleo de Domínio
- [ ] Develop 3 — Integração e API
- [ ] Develop 4 — Docker e Publicação
