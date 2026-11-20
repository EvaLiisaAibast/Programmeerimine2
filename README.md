# Epood – ASP.NET Core Web API E-Shop

Epood (Estonian for "e-shop") is a shop backend built as coursework for **Programmeerimine 2**. It is a work in progress: every feature is implemented, but the schema and the endpoints may still change.

The project is split into an application layer (`KooliProjekt.Application`) and a Web API layer (`KooliProjekt.WebAPI`). Requests flow through MediatR, every command and query has its own handler and FluentValidation validator, and every endpoint returns an `OperationResult` wrapper.

## Tech Stack

| Area | Technology |
| --- | --- |
| Runtime | .NET 10 (`net10.0`), ASP.NET Core Web API |
| Data access | Entity Framework Core 10.0.12, SQL Server |
| Application layer | MediatR 12.5.0 (CQRS), FluentValidation 12.1.1 |
| Authentication | JWT Bearer 10.0.12, `System.IdentityModel.Tokens.Jwt` |
| Documentation | Swashbuckle 10.2.3 (Swagger UI with Bearer support) |
| Serialization | Newtonsoft.Json with camelCase properties |
| Password hashing | PBKDF2-SHA256 from `System.Security.Cryptography` |

## Current State

- **56 `DbSet`s** in `ApplicationDbContext`, modelled in `Data/FullSchemeEntities.cs`
- **57 entity controllers** with a uniform CRUD surface
- **JWT authentication** with a login endpoint
- **Role-based authorization**: admins receive the `Admin` role claim
- **PBKDF2 password hashing** with a random salt and Base64 storage
- **Local file storage** for product images and product files, served from `/files`
- **Swagger UI** with an `Authorize` button, so protected endpoints can be tested from the browser
- One-time **seed data**: 36 sample rows per table plus two login accounts

## API Conventions

Every entity controller derives from `ApiControllerBase`, so every entity exposes the same routes:

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/{entity}/get/{id}` | One item by id |
| `GET` | `/api/{entity}/list/{page}/{pageSize}` | Paged list |
| `POST` | `/api/{entity}/save` | Create (id `0`) or update (id `> 0`) |
| `DELETE` | `/api/{entity}/delete/{id}` | Delete by id |

Examples: `/api/brands/get/1`, `/api/products/list/1/10`, `/api/users/save`.

`ProductFilesController` is the only controller with a file-specific surface. It stores the upload through the local file client and serves the stored file back:

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/api/productfiles/add/{id}` | Upload a file for product `id` (multipart form: `file`, optional `altText`, `sortOrder`, `isMain`) |
| `GET` | `/api/productfiles/get/{id}` | Download the file with that id |
| `GET` | `/api/productfiles/list/{id}` | List the files of product `id` |
| `DELETE` | `/api/productfiles/delete/{id}` | Delete a file |

Uploaded files are written to the folder configured by `FilesRoot` and are also reachable as static content under `/files`.

All responses are wrapped in an `OperationResult`: successful calls return `{ "value": ... }`, failed validation returns HTTP 400 with a `errors` object.

## Authentication and Authorization

Authorization is required by default. `ApiControllerBase` carries `[Authorize]`, so every endpoint except the login endpoint returns **401 Unauthorized** without a valid token.

1. **Log in**

   ```http
   POST /api/authentication/authenticate
   Content-Type: application/json

   { "userName": "admin", "password": "Admin123!" }
   ```

   The response contains a JWT: `{ "value": "<token>" }`.

2. **Call a protected endpoint**

   ```http
   GET /api/brands/list/1/10
   Authorization: Bearer <token>
   ```

3. **Test it in Swagger** — open `/swagger`, click **Authorize**, paste the token *without* the word `Bearer`, and execute the request.

Details:

- Tokens are signed with **HMAC-SHA256**, issued by `KooliProjekt`, expire after **60 minutes**, and carry the `nameid`, `unique_name` and — for administrators — `role` claims.
- The role claim is only added when the user's `IsAdmin` flag is `true`, which makes `[Authorize(Roles = "Admin")]` usable for future admin-only endpoints.
- Two accounts are seeded on the first run: `admin` / `Admin123!` (administrator) and `kasutaja` / `Kasutaja123!` (regular user).

## Password Hashing

Passwords are never stored in plain text. `PasswordHelper` uses:

- **PBKDF2 with SHA-256**, 100 000 iterations
- a cryptographically random **16-byte salt** generated per password
- a **32-byte derived key**
- **Base64** encoding of `salt + key` for the database column
- `CryptographicOperations.FixedTimeEquals` when verifying, so the comparison does not leak timing information

Verification fails safely (returns `false`) for malformed Base64 or an unexpected stored length.

## Configuration

`KooliProjekt.WebAPI/appsettings.json` holds:

| Key | Purpose |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `Jwt:Key` | Signing key for the JWT |
| `Jwt:Issuer`, `Jwt:Audience` | Token issuer and audience (`KooliProjekt`) |
| `Jwt:ExpireMinutes` | Token lifetime in minutes (`60`) |
| `FilesRoot` | Folder for uploaded files (`c:\temp\uploads\`) |

The values in `appsettings.json` are coursework defaults. In a real deployment the signing key belongs in user secrets, environment variables or a secret store — never in a public repository.

## Database

- EF Core migrations live in `KooliProjekt.Application/Migrations`
- `AddUsersTable` adds the `Users` table with a unique `UserName` index and the `IsAdmin` flag
- Migrations run automatically on startup, followed by the one-time seed (`SeedData`), which fills 36 rows per table and creates the two login accounts

## Project Structure

```
.
├── KooliProjekt.Application/       # Domain entities, DbContext, features, validators, migrations
│   ├── Authentication/             # JwtSettings, JwtTokenBuilder, PasswordHelper
│   ├── Data/                       # Entities, ApplicationDbContext, SeedData
│   ├── Dto/                        # Data transfer objects
│   ├── Features/                   # One folder per feature: commands, queries, handlers, validators
│   ├── Infrastructure/             # OperationResult, file client, user provider, behaviors
│   └── Migrations/
├── KooliProjekt.WebAPI/            # ASP.NET Core Web API (startup project)
│   ├── Controllers/                # One controller per entity
│   ├── wwwroot/                    # Static files, placeholder images
│   ├── WebUserProvider.cs          # Reads the current user from the JWT
│   └── Program.cs                  # DI, authentication, Swagger, startup
├── KooliProjekt.sln
├── EPoodDiagram.png / .svg         # Full database diagram
├── EPoodDiagramSimple.png / .svg   # Simplified database diagram
└── EPoodModel.docx                 # Data model documentation
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express or a full instance)
- Optional: `dotnet tool install --global dotnet-ef`

### Steps

1. **Clone the repository**

   ```bash
   git clone https://github.com/EvaLiisaAibast/Programmeerimine2.git
   cd Programmeerimine2
   ```

2. **Set the connection string** in `KooliProjekt.WebAPI/appsettings.json`

3. **Build and run**

   ```bash
   dotnet build KooliProjekt.sln
   dotnet run --project KooliProjekt.WebAPI
   ```

   The API starts on `http://localhost:5086` and Swagger UI opens at `http://localhost:5086/swagger`.

4. **Log in and try a protected endpoint** as described in [Authentication and Authorization](#authentication-and-authorization).

Migrations and the seed run automatically on startup, so no separate `dotnet ef database update` step is needed for a fresh database.

## Database Diagrams

| Diagram | Files |
| --- | --- |
| Full model | `EPoodDiagram.png`, `EPoodDiagram.svg` |
| Simplified model | `EPoodDiagramSimple.png`, `EPoodDiagramSimple.svg` |

The complete model description is available in `EPoodModel.docx`.

---

# Eesti keeles

**Epood** on *Programmeerimine 2* kursuse raames tehtud ASP.NET Core Web API pood. Projekt on jagatud rakenduskihiks (`KooliProjekt.Application`) ja veebi-API kihiks (`KooliProjekt.WebAPI`). Iga käsk ja päring läbib MediatR-i, igal on oma handler ja FluentValidation validaator, ning kõik vastused on `OperationResult` ümbrises.

**Mis on praegu valmis:**

- 56 olemit ja 57 kontrollerit ühtse CRUD-otspunktidega
- **JWT autentimine** (`POST /api/authentication/authenticate`) — token kehtib 60 minutit
- **Autoriseerimine**: `ApiControllerBase` kannab `[Authorize]`-i, seega ilma tokenita vastab iga otspunkt **401**-ga; ainus avalik otspunkt on sisselogimine
- **Rollipõhine juurdepääs**: administraator saab `Admin` rolli claimi (`IsAdmin` veeru järgi), tavakasutaja mitte
- **Paroolide hashimine PBKDF2-SHA256-ga** (100 000 iteratsiooni, 16-baidine juhuslik sool, 32-baidine räsi, base64, võrdlemine `FixedTimeEquals`-iga)
- **Failihoidla** toodete piltidele ja failidele (kaust `FilesRoot`, serveeritakse `/files` alt)
- **Swagger UI koos „Authorize" nupuga** — token kleepida ilma sõnata `Bearer`
- Migratsioonid ja seemendamine jooksevad käivitamisel automaatselt

**Testkasutajad (seemendatakse esimesel käivitamisel):**

| Kasutajanimi | Parool | Roll |
| --- | --- | --- |
| `admin` | `Admin123!` | administraator (`IsAdmin = true`) |
| `kasutaja` | `Kasutaja123!` | tavakasutaja |

**Kiire proovimine:**

```bash
dotnet run --project KooliProjekt.WebAPI
# Swagger: http://localhost:5086/swagger
# 1) POST /api/authentication/authenticate  {"userName":"admin","password":"Admin123!"}
# 2) Authorize -> kleebi saadud token
# 3) GET /api/users/list/1/10  -> 200
#    ilma tokenita sama päring -> 401
```

**NB:** `appsettings.json`-is olevad väärtused (sh `Jwt:Key`) on mõeldud kursusetöö jaoks. Päris juurutuses peab allkirjastamise võti olema user secrets / keskkonnamuutujates / secret manageris, mitte avalikus repos.

---

## Status

Work in progress — features, endpoints and the schema may still change.

## Author

[EvaLiisaAibast](https://github.com/EvaLiisaAibast)
