# CLAUDE.md

Task management system: an ASP.NET Core 8 Web API with PostgreSQL, a React client in a separate repo, and deployment with Docker and Render.

## The spec is the source of truth

- The analysis (in Hebrew) is in `Task.api/claude/analysis/`. Start with `00-index.md`, which maps the documents and the ID prefixes (EN, DT, CO, FN, AP, GN, DP, TC, ...).
- `dev-tasks.md` lists 49 tasks in order. A task is closed only when its done criterion ("סיום") is met. Deployment tasks 46–49 run before task 45.
- Implement exactly what the referenced rules say. If the spec is ambiguous or contradicts itself, ask the user. Do not invent behavior.
- When code implements a rule, cite the rule ID in a short comment, the way `docker-compose.yml` and `db/init.sql` do (e.g. `// GN-12`).

## Workflow

- **Get the user's approval before any change or decision.** Before editing files, running commands that change state (git commit/push, migrations, docker, package installs), or making any decision the spec does not dictate (naming, structure, libraries, deviations, workarounds), describe what you intend to do and why, then wait for explicit approval. Read-only checks are fine without asking. If you find something unexpected while working, stop and ask; do not fix it on your own.
- Before starting a task, tell the user which one it is: number, name, details, rule references, and done criterion from `dev-tasks.md`. Then do the work.
- Do one task at a time. When it's done, check the done criterion (build, tests, or a manual check), then commit with a message like `<Summary> (task N)`, and push to `main`.
- `main` is the deployment branch (DP-03-R1).

## Repositories

| Local | GitHub |
|---|---|
| `D:\lali fruchter\Task` (this repo, server) | https://github.com/lali507/tasks-api |
| `D:\lali fruchter\Task-client` (React client) | https://github.com/lali507/tasks-client |

Git's OpenSSL fails on this network ("unable to get local issuer certificate"). Both repos set `http.sslBackend=schannel` locally. Keep that setting.

## Solution layout

| Project | Namespace | References |
|---|---|---|
| `Task.Core` | `TaskManager.Core` | none: no projects and no packages (CO-03) |
| `Task.Data` | `TaskManager.Data` | Core |
| `Task.Service` | `TaskManager.Service` | Core |
| `Task.api` | `TaskManager.Api` | Core, Data, Service |
| `Task.Tests` | `TaskManager.Tests` | all (xUnit) |

- Repository and service interfaces live in Core (CO-04). Service depends on those interfaces, never on Data.
- **Never use a namespace that starts with `Task`.** A `Task` namespace hides `System.Threading.Tasks.Task`, so a plain `async Task` fails with CS0118. Project and folder names stay `Task.*`; namespaces are `TaskManager.*`.
- Every async method takes a `CancellationToken` (CO-04).
- Target framework: `net8.0`. Nullable and ImplicitUsings are enabled. Stay on .NET 8. Do not suggest upgrading.

## Modern code and up-to-date libraries

- Use current, idiomatic C# 12 syntax: file-scoped namespaces, collection expressions (`[]`), primary constructors, pattern matching, `required` members, and raw string literals where they help.
- Use the latest stable versions of NuGet and npm packages that support .NET 8. Do not use deprecated packages or APIs.
- Before adding a library, propose it (name, version, and why) and wait for approval.

## Commands

```bash
docker compose up -d db           # PostgreSQL 16 (container taskdb): TaskDb + TaskDb_Test
docker compose up -d --build      # db + api (container taskapi, http://localhost:8080; secrets from .env)
dotnet build Task.slnx
dotnet test Task.slnx
dotnet run --project Task.api
dotnet user-secrets list --project Task.api

# EF Core migrations (dotnet-ef is a local tool: run `dotnet tool restore` once after cloning)
dotnet ef migrations add <Name> --project Task.Data --startup-project Task.api
dotnet ef database update --project Task.Data --startup-project Task.api
```

## Deviations from the spec

Approved by the user. List each one in the README (task 45).

| Rule | Spec says | Implemented | Why |
|---|---|---|---|
| SV-01, AP-05 | AutoMapper Profiles, registered in Program.cs | Mapperly (`Riok.Mapperly`) static mappers in `Task.Service/Mapping/`, no DI registration | Free AutoMapper (≤14.0.0, MIT) has a High vulnerability (GHSA-rvv3-g6hj-g44x); fixed versions (15+) need a commercial license |

## Secrets (IN-01, DP-02-R1, DP-03-R3)

- Never commit secrets. `.env` is local only; `.env.example` (keys without values) is committed.
- In development, `ConnectionStrings:DefaultConnection`, `ConnectionStrings:TestConnection`, and `Jwt:Key` are in User Secrets. In deployment they come from environment variables (DP-04-R1).
- `appsettings.json` contains no secrets.
- PostgreSQL runs in Docker, never as a local install (IN-03).
