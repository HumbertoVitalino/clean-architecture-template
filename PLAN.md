# Plano de Implementação

Cada **Develop** é um ciclo de entrega que agrupa features relacionadas.
O fluxo de cada ciclo é:

```
feature/X-slug  →  PR → develop  →  CI passa  →  PR automático → release
```

---

## Develop 1 — Fundação

> Objetivo: projeto buildável, CI/CD no ar, template config funcionando.
> Sem essa base nada mais pode ser validado de forma confiável.

| # | Branch                        | Entrega                                                              |
|---|-------------------------------|----------------------------------------------------------------------|
| 1 | `feature/1-solution-structure`| Mover para `src/` e `tests/`, renomear projetos, `Directory.Build.props` |
| 2 | `feature/2-github-actions`    | `ci-feature.yml`, `ci-develop.yml` (com PR automático), `codeql.yml`, `cd.yml` |
| 3 | `feature/3-template-config`   | `.template.config/template.json` com `sourceName` e símbolos `useControllers` / `useMediatR` |

**Critério de saída:** `dotnet new clean-arch -n Test.App` gera a solução com os nomes substituídos e a pipeline CI passa no push para `develop`.

---

## Develop 2 — Núcleo de Domínio

> Objetivo: camadas Domain e Application implementadas com exemplo funcional.
> Testes unitários já validando o comportamento.

| # | Branch                     | Entrega                                                                                   |
|---|----------------------------|-------------------------------------------------------------------------------------------|
| 4 | `feature/4-domain-layer`   | `AggregateRoot<TId>`, `ValueObject`, `IDomainEvent`, `IRepository<T,TId>`, `Result<T>` / `Error`, entidade `User` + `Email` VO + `UserCreatedEvent` |
| 5 | `feature/5-application-layer` | `IUseCase<TInput, TOutput>`, `CreateUserUseCase` (padrão sem MediatR), variante `#if useMediatR` com `CreateUserCommand` + Handler, DTOs, `ApplicationServiceExtensions` |
| 6 | `feature/6-unit-tests`     | Projeto `UnitTests`, testes AAA para `User` aggregate e `CreateUserUseCase` com NSubstitute |

**Critério de saída:** `dotnet test --filter Category=Unit` passa 100% na pipeline de feature e develop.

---

## Develop 3 — Integração e API

> Objetivo: banco, repositório e API funcionando de ponta a ponta.
> Testes de integração e funcionais validando o fluxo completo.

| # | Branch                          | Entrega                                                                                    |
|---|---------------------------------|--------------------------------------------------------------------------------------------|
| 7 | `feature/7-infrastructure-layer`| `AppDbContext` (SQL Server), `UserRepository`, dispatch de domain events no `SaveChangesAsync`, migrations, `InfrastructureServiceExtensions` |
| 8 | `feature/8-api-layer`           | Minimal API endpoints (padrão), variante Controllers `#if useControllers`, JWT Bearer, OpenAPI, `ProblemDetails`, `ApiServiceExtensions` |
| 9 | `feature/9-integration-tests`   | Projeto `IntegrationTests` com Testcontainers SQL Server, `UserRepositoryTests`            |
| 10| `feature/10-functional-tests`   | Projeto `FunctionalTests` com `CustomWebApplicationFactory`, `UsersEndpointTests`          |

**Critério de saída:** `dotnet test` (todos os projetos) passa na pipeline de develop. API sobe localmente com `docker compose up`.

---

## Develop 4 — Docker e Publicação

> Objetivo: imagem Docker pronta e template publicável no NuGet.
> Entrega final — o que o usuário do template vai consumir.

| #  | Branch                  | Entrega                                                                                      |
|----|-------------------------|----------------------------------------------------------------------------------------------|
| 11 | `feature/11-docker`     | `Dockerfile` multi-stage (non-root, health check), `docker-compose.yml` (api + sqlserver + migrate), `docker-compose.override.yml`, `.env.example` |
| 12 | `feature/12-nuget-publish` | Configuração de empacotamento, `README.md` completo com badges e exemplos de uso, validação do `dotnet pack` + install local |

**Critério de saída:** `docker compose up` sobe o stack completo. `dotnet new install ./` + `dotnet new clean-arch -n MinhaEmpresa.MeuProduto` gera projeto funcional.

---

## Resumo Visual

```
release ◄────────────────────────────────────────────────────────────────────────────
           PR automático   PR automático    PR automático    PR automático
develop ◄──────────────── ◄──────────────── ◄──────────────── ◄────────────────────
        feat/1  feat/2  feat/3  feat/4  feat/5  feat/6  feat/7  feat/8 ... feat/12
        [  Develop 1   ]  [    Develop 2    ]  [      Develop 3      ]  [ Develop 4 ]
```

---

## Estado

| Develop   | Status        | Observação                                              |
|-----------|---------------|---------------------------------------------------------|
| Develop 1 | `done`        | features 1–3 merged em develop e release                |
| Develop 2 | `done`        | features 4–6 merged em develop                          |
| Develop 3 | `in progress` | feature/7 merged; features 8–10 pendentes               |
| Develop 4 | `pending`     | aguarda Develop 3                                       |
