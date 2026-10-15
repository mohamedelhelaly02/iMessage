# iMessage

A real-time chat application in progress, built as an Angular client and an ASP.NET Core API. The project brings together account authentication, private and group conversations, online presence, and typing indicators in a clean, iMessage-inspired interface.

> **Project status:** Conversation setup and real-time presence/typing are implemented. The message list currently displays sample content, and the composer is not connected to message storage or delivery yet.

## Features

- **Account access** — register and sign in with email and password, protected routes, JWT access tokens, and rotating refresh tokens stored in an HTTP-only cookie.
- **Private and group conversations** — create one-to-one conversations or groups and load the signed-in user's conversation list.
- **Live presence** — SignalR reports online users, offline users, and last-seen information to conversation contacts.
- **Typing indicators** — real-time typing and stopped-typing events, scoped to conversation participants.
- **Chat interface** — Angular conversation list, conversation view, message composer, empty state, and toast notifications.
- **Input validation and API errors** — FluentValidation request validation and centralized API exception handling.

## Technology stack

| Area | Technologies |
| --- | --- |
| Frontend | Angular 22, TypeScript 6, RxJS, Angular Router |
| Real-time client | Microsoft SignalR JavaScript client |
| UI utilities | ngx-translate, ngx-sonner, custom CSS |
| Backend | ASP.NET Core / .NET 10, Minimal APIs, SignalR |
| Application patterns | MediatR, FluentValidation |
| Identity and authentication | ASP.NET Core Identity, JWT bearer authentication |
| Persistence | Entity Framework Core 10, SQL Server |

## How it fits together

```text
Angular client (HTTPS, localhost:4200)
  ├── REST calls ───────────────> ASP.NET Core API (/api)
  └── SignalR connection ───────> Chat hub (/hubs/chat)
                                      │
                                      ├── JWT authentication
                                      └── Entity Framework Core ──> SQL Server
```

The client is organized by feature, with shared layouts and core services. The server separates domain entities, application handlers and hub behavior, infrastructure (Identity and persistence), and shared API concerns.

## Getting started

### Prerequisites

- .NET 10 SDK
- Node.js and npm (the client declares npm `11.19.0`)
- SQL Server, reachable with the connection string you configure
- The `dotnet-ef` tool for applying Entity Framework migrations

### 1. Configure the backend

From the repository root, set the database connection and JWT settings with .NET user secrets rather than committing credentials:

```powershell
dotnet user-secrets set "ConnectionStrings:Default" "Server=.;Database=iMessage;Integrated Security=true;TrustServerCertificate=True" --project .\server\server.csproj
dotnet user-secrets set "JwtSettings:Secret" "replace-with-a-long-random-secret-value" --project .\server\server.csproj
dotnet user-secrets set "JwtSettings:Issuer" "iMessage" --project .\server\server.csproj
dotnet user-secrets set "JwtSettings:Audience" "iMessage-client" --project .\server\server.csproj
dotnet user-secrets set "JwtSettings:ExpirationInMinutes" "5" --project .\server\server.csproj
dotnet user-secrets set "JwtSettings:RefreshTokenExpirationInDays" "7" --project .\server\server.csproj
```

The API expects a `JwtSettings` configuration section. Supply it through user secrets as above; keep secret values out of source control. The configured Angular origin is `https://localhost:4200`.

Apply the database migrations:

```powershell
dotnet ef database update --project .\server\server.csproj
```

If `dotnet ef` is not installed, install the .NET 10 Entity Framework CLI tool first:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.12
```

### 2. Start the API

In a PowerShell terminal at the repository root:

```powershell
$env:ASPNETCORE_URLS = "https://localhost:7005"
dotnet run --project .\server\server.csproj
```

If the local HTTPS development certificate is not trusted yet, run `dotnet dev-certs https --trust` and restart the API.

### 3. Start the client

In a second terminal:

```powershell
Set-Location .\client
npm ci
npm run start:dev
```

The development client uses `https://localhost:7005` as its API URL and serves over HTTPS on port `4200`.

## Useful commands

Run these from the `client` directory:

| Command | Purpose |
| --- | --- |
| `npm run start:dev` | Start the Angular development server |
| `npm run build` | Build the client |
| `npm test` | Run client tests |

Run these from the repository root:

| Command | Purpose |
| --- | --- |
| `dotnet build .\server\server.csproj` | Build the API |
| `dotnet ef database update --project .\server\server.csproj` | Apply database migrations |

## Project structure

```text
client/
  src/app/
    core/       Guards, layouts, interceptors, and shared services
    features/   Authentication, chat, and user features
    shared/     Reusable UI components
  public/       Static assets
server/
  Application/ MediatR handlers, validation, and SignalR hub
  Domain/      Users, conversations, participants, and refresh tokens
  Infrastructure/ Identity, EF Core persistence, migrations, and seeding
  Shared/      API results, endpoint helpers, and exception handling
```

## Current scope

The API supports registration, login/logout, token refresh, user lookup, and private/group conversation creation and retrieval. The SignalR hub handles authenticated connections, presence, conversation membership, and typing events.

Messages themselves are not yet backed by an API or database model. The current conversation view uses sample messages, and submitting the composer emits a client-side event only. Message delivery and history are future work.