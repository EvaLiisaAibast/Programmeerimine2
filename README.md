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
- **57 entity controllers** with a uniform CRUD surface, all restricted to administrators
- **JWT authentication** with a login endpoint
- **Role-based authorization**: admins receive the `Admin` role claim
- **Public API** in `Features/Public` with everything an end user can do: registration, profile, cart, checkout, order history, payments, notifications, reviews and returns
- **PBKDF2 password hashing** with a random salt and Base64 storage
- **Local file storage** for product images and product files, served from `/files`
- **Swagger UI** with an `Authorize` button, so protected endpoints can be tested from the browser
- One-time **seed data**: 36 sample rows per table plus two login accounts

## API Conventions

Every entity controller derives from `AdminApiBaseController` (which itself derives from `ApiControllerBase`), so every entity exposes the same routes and requires the `Admin` role:

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

## Public API (End User)

Everything the end user does lives in the `Public` feature (`KooliProjekt.Application/Features/Public`) and is exposed by `PublicController`. Every handler resolves the signed in user through `CurrentCustomer`, so it can only read or write data that belongs to that user.

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/api/public/register` | Register a new customer (creates the customer profile and the login account) |
| `GET` | `/api/public/products` | Active products; paging, `search`, `categoryId` |
| `GET` | `/api/public/products/{id}` | Product detail with variants, images and approved reviews |
| `GET` | `/api/public/categories` | Categories with the number of active products |
| `GET` | `/api/public/profile` | Own profile with addresses |
| `POST` | `/api/public/profile` | Change own profile data |
| `POST` | `/api/public/profile/password` | Change own password |
| `POST` | `/api/public/profile/addresses` | Add a new address or change an existing one |
| `GET` | `/api/public/cart` | Own cart with prices, totals and stock |
| `POST` | `/api/public/cart/items` | Add a product to the cart |
| `PUT` | `/api/public/cart/items/{cartItemId}?quantity=2` | Change the quantity of one cart line |
| `DELETE` | `/api/public/cart/items/{cartItemId}` | Remove a cart line |
| `GET` | `/api/public/wishlist` | Own wishlist with products, prices and stock |
| `POST` | `/api/public/wishlist/items` | Add a product to the wishlist |
| `DELETE` | `/api/public/wishlist/items/{wishlistItemId}` | Remove a wishlist line |
| `POST` | `/api/public/orders` | Checkout: turn the cart into an order |
| `GET` | `/api/public/orders` | Own order history |
| `GET` | `/api/public/orders/{id}` | One own order with lines, payments and shipment |
| `GET` | `/api/public/payments` | Own payments and invoices |
| `GET` | `/api/public/notifications` | Own notifications (`unreadOnly=true` for unread ones) |
| `POST` | `/api/public/notifications/{id}/read` | Mark one own notification as read |
| `POST` | `/api/public/reviews` | Review a product from a delivered order |
| `POST` | `/api/public/returns` | Request a return for an order line |
| `GET` | `/api/public/returns` | Own return requests |

Registration, the product catalog and the category list are `[AllowAnonymous]`. All other public endpoints need a token and answer **401** without one.

Rules the handlers enforce:

- the customer is resolved from the token on every request, so a user can neither read nor change another customer's cart, orders, payments, notifications or returns — such requests answer `"Tellimust ei leitud."` or `"Ostukorvi rida ei leitud."`
- adding a product that is already in the cart is refused, with a hint to change the quantity instead
- the wishlist works the same way: a product that is already on it is refused, and a wishlist line of another customer answers `"Soovinimekirja rida ei leitud."`; the first wishlist is created automatically when the customer adds the first product
- quantities are checked against the stock levels, and the checkout re-checks that every product is still active and in stock
- checkout needs at least one cart line and an address (the default address is used when `addressId` is `0`)
- a blocked customer (`Status != "Aktiivne"`) can browse the catalog but cannot add to the cart or place an order
- a review is only possible for a delivered order line, and only once per line
- a return can never exceed the ordered quantity that has not been returned yet

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
- The role claim is only added when the user's `IsAdmin` flag is `true`. `AdminApiBaseController` carries `[Authorize(Roles = "Admin")]`, so the `/api/{entity}` routes answer **403** for a regular user.
- Two accounts are seeded on the first run: `admin` / `Admin123!` (administrator) and `kasutaja` / `Kasutaja123!` (regular user). Both are linked to an active customer profile, which is what the public API works with.

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
- `AddCustomerIdToUser` links a login account to its customer profile (`Users.CustomerId`), because the login accounts and the shop customers are separate tables
- Migrations run automatically on startup, followed by the one-time seed (`SeedData`), which fills 36 rows per table, creates the two login accounts and links them to customer profiles

## Project Structure

```
.
├── KooliProjekt.Application/       # Domain entities, DbContext, features, validators, migrations
│   ├── Authentication/             # JwtSettings, JwtTokenBuilder, PasswordHelper
│   ├── Data/                       # Entities, ApplicationDbContext, SeedData
│   ├── Dto/                        # Data transfer objects
│   ├── Features/                   # One folder per feature: commands, queries, handlers, validators
│   │   └── Public/                 # End-user endpoints, DTOs and the current-customer resolver
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
- **`AdminApiBaseController`** — kõik olemikontrollerid pärinevad sellest ja nõuavad `Admin` rolli (tavakasutajale vastus **403**)
- **Public feature ja `PublicController`** (`/api/public/...`) — kõik lõppkasutaja tegevused: registreerimine, profiil ja aadressid, ostukorv, soovinimekiri, tellimuse esitamine, tellimuste ajalugu, maksed, teavitused, arvustused ja tagastused
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

**Avalik API lõppkasutajale** (`/api/public/...`): `register`, `products`, `categories` on anonüümsed; kõik ülejäänud nõuavad tokenit. Iga handler tuvastab kasutaja tokenist ja puutub ainult tema enda andmeid — teise kliendi tellimus vastab `"Tellimust ei leitud."`, teise kliendi ostukorvi rida `"Ostukorvi rida ei leitud."`. Tingimused, mida handlerid kontrollivad: juba ostukorvis olev toode, laoseis, tühi ostukorv, puuduv aadress, blokeeritud konto, arvustus ainult kättesaadud tellimusele ja üks kord rea kohta, tagastus ei tohi ületada tellitud kogust.

**NB:** `appsettings.json`-is olevad väärtused (sh `Jwt:Key`) on mõeldud kursusetöö jaoks. Päris juurutuses peab allkirjastamise võti olema user secrets / keskkonnamuutujates / secret manageris, mitte avalikus repos.

---

## Status

Work in progress — features, endpoints and the schema may still change.

## Author

[EvaLiisaAibast](https://github.com/EvaLiisaAibast)
