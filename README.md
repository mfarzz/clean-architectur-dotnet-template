# .NET 10 Clean Architecture Monolith Starter Template (`cleanapi`)

Template starter .NET 10 Web API berbasis **Clean Architecture** (Monolith) dengan pemisahan tanggung jawab yang disiplin, pola CQRS via MediatR, validasi otomatis, *global exception handling* standar RFC 7807 (`ProblemDetails`), *structured logging* menggunakan Serilog, dokumentasi OpenAPI/Scalar, serta fleksibilitas memilih **Database (PostgreSQL / SQL Server)** dan **Gaya API (Controllers / Minimal APIs)**.

---

## 🏛️ Arsitektur Proyek

Solusi ini diorganisasikan ke dalam 4 layer utama sesuai prinsip *Clean Architecture* dan *Dependency Inversion*:

```text
├── src/
│   ├── MyCleanApi.Domain/          # Inti bisnis murni (Entities, Result Pattern, Value Objects) - Nol dependencies eksternal
│   ├── MyCleanApi.Application/     # CQRS Features (Commands, Queries, Validators, Handlers, Pipeline Behaviors)
│   ├── MyCleanApi.Infrastructure/  # EF Core DbContext, Interceptors, Repositories, Database Providers
│   └── MyCleanApi.WebApi/          # API Entry Point (Controllers & Minimal APIs, Middlewares, Program.cs)
├── tests/
│   ├── MyCleanApi.Domain.UnitTests/
│   ├── MyCleanApi.Application.UnitTests/
│   └── MyCleanApi.WebApi.IntegrationTests/
├── .template.config/               # Metadata template 'dotnet new'
├── Directory.Build.props           # Konfigurasi global MSBuild (Target net10.0, Nullable, CPM)
├── Directory.Packages.props        # Central Package Management (Versi NuGet terpusat)
└── docker-compose.yml              # Local database orchestration (PostgreSQL & SQL Server)
```

### Aturan Dependensi (Dependency Rule)
- **Domain**: Tidak bergantung pada layer mana pun dan tidak memiliki package eksternal.
- **Application**: Hanya bergantung pada **Domain**.
- **Infrastructure**: Bergantung pada **Application** dan **Domain**.
- **WebApi**: Entry point yang mereferensikan **Application** dan **Infrastructure**.

---

## 🚀 Fitur Utama & Tech Stack

- **Framework**: .NET 10 (`net10.0`)
- **CQRS & MediatR**: Pemisahan perintah (*Write*) dan kueri (*Read*) menggunakan *Feature Folders*.
- **Result Pattern**: Menghindari melempar exception untuk *flow control* bisnis (`Result`, `Result<T>`, `Error`).
- **Validation Pipeline**: FluentValidation terintegrasi otomatis melalui MediatR `IPipelineBehavior`.
- **Global Error Handling**: Standardisasi respon error menggunakan RFC 7807 (`ProblemDetails`) via `IExceptionHandler`.
- **Structured Logging**: Serilog dikonfigurasi ke Console dan File harian berputar (*rolling file*).
- **API Documentation**: Scalar UI & Microsoft OpenAPI modern.
- **Multi-Database Support**: Beralih antara PostgreSQL dan SQL Server cukup dengan mengganti `"DatabaseProvider"` di `appsettings.json`.
- **Dual API Style Support**: Dilengkapi contoh **Controllers** (`SampleItemsController`) dan **Minimal APIs** (`SampleItemEndpoints`).
- **Automated Auditing**: EF Core `AuditableEntityInterceptor` otomatis mengisi `CreatedAt`, `CreatedBy`, `UpdatedAt`, dan `UpdatedBy`.
- **Central Package Management (CPM)**: Seluruh versi paket NuGet dikelola terpusat di `Directory.Packages.props`.

---

## 📦 Cara Memakai Sebagai Custom `dotnet new` Template

### 1. Instal Template Secara Lokal
Jalankan perintah ini di root repositori:
```bash
dotnet new install .
```

### 2. Generate Proyek Baru
Gunakan perintah `dotnet new cleanapi` dengan nama proyek Anda:

#### Opsi 1: Proyek dengan Controllers & PostgreSQL (Default)
```bash
dotnet new cleanapi -n TokoOnline
```

#### Opsi 2: Proyek dengan Minimal APIs & SQL Server
```bash
dotnet new cleanapi -n FinTechService --db sqlserver --api-style minimal
```

Parameter yang tersedia:
- `-n, --name <NAME>` : Nama solution, namespace, dan project baru (menggantikan `MyCleanApi` secara otomatis).
- `--db <postgres|sqlserver>` : Pilihan database provider.
- `--api-style <controllers|minimal>` : Pilihan gaya endpoint API.

### 3. Uninstall Template
Jika ingin menghapus instalasi template dari mesin Anda:
```bash
dotnet new uninstall .
```

---

## 💻 Menjalankan Proyek Secara Lokal

### 1. Nyalakan Database Lokal via Docker
Pastikan Docker Desktop aktif, lalu jalankan:
```bash
docker compose up -d
```
*Secara default service PostgreSQL di port `5432` akan dinyalakan.*

### 2. Jalankan Web API
```bash
dotnet run --project src/MyCleanApi.WebApi
```

Buka browser dan akses dokumentasi API interaktif Scalar di:
- **Scalar UI**: `https://localhost:7123/scalar/v1` (atau port HTTP lokal Anda)
- **Health Checks**: `https://localhost:7123/health`

---

## 🧪 Menjalankan Pengujian (Testing)

Proyek ini telah dilengkapi dengan Unit Tests dan Integration Tests:
```bash
dotnet test
```

Cakupan pengujian meliputi:
- **Domain Unit Tests**: Logika domain entity, value object, dan Result pattern.
- **Application Unit Tests**: Validasi FluentValidation dan CQRS Handler dengan NSubstitute mocking.
- **WebApi Integration Tests**: Pengujian endpoint menyeluruh menggunakan `WebApplicationFactory` dan SQLite/InMemory database terisolasi.

---

## 📄 Lisensi
Proyek template ini didistribusikan di bawah lisensi MIT.
