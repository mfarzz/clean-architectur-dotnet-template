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
- **API Documentation**: Swagger UI, Scalar UI & Microsoft OpenAPI modern.
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
- `-D, --db <postgres|sqlserver>` : Pilihan database provider. Mengatur `DatabaseProvider`, connection string, dan service Docker Compose pada proyek hasil generate.
- `--api-style <controllers|minimal>` : Pilihan gaya endpoint API.

Contoh SQL Server dengan short flag:
```bash
dotnet new cleanapi -n ECommerce -D sqlserver
```

Setelah generate, sesuaikan connection string dan password database untuk lingkungan Anda. Perubahan template tidak diterapkan otomatis ke proyek yang sudah dibuat.

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
*Pada proyek hasil generate, service mengikuti pilihan `--db`: PostgreSQL di port `5432` atau SQL Server di port `1433`. Saat menjalankan source repository ini langsung, gunakan `docker compose -f docker-compose.sqlserver.yml up -d` untuk SQL Server dan sesuaikan konfigurasi database.*

### 2. Jalankan Web API
```bash
dotnet run --project src/MyCleanApi.WebApi
```

Buka browser dan akses dokumentasi API (aktif hanya pada environment `Development`):
- **Swagger UI**: `http://localhost:5264/swagger`
- **Scalar UI**: `http://localhost:5264/scalar/v1`
- **OpenAPI JSON**: `http://localhost:5264/openapi/v1.json`
- **Health Checks**: `http://localhost:5264/health`

Swagger UI dan Scalar menggunakan dokumen OpenAPI yang sama. Jika memakai profil `https`, gunakan `https://localhost:7255`.

Swagger UI menyediakan tema **System / Light / Dark**. Secara default tema mengikuti preferensi browser/OS dan berubah otomatis saat preferensi sistem berubah. Pilihan tema manual disimpan di browser.

Klik **Authorize**, lalu masukkan access token tanpa awalan `Bearer`. Swagger akan mengirim header `Authorization: Bearer <token>` saat mencoba endpoint. Token tidak disimpan permanen oleh Swagger. Endpoint contoh tetap publik; fitur ini hanya konfigurasi dokumentasi, bukan implementasi login atau validasi JWT. Untuk mengamankan endpoint, konfigurasi autentikasi JWT dan middleware-nya terlebih dahulu, lalu gunakan `[Authorize]` atau `.RequireAuthorization()`.

---

## 🧪 Menjalankan Pengujian (Testing)

Proyek ini telah dilengkapi dengan Unit Tests dan Integration Tests:
```bash
dotnet test
```

Khusus source repository template, verifikasi paket dengan generate proyek PostgreSQL dan SQL Server, lalu jalankan build dan pengujiannya:
```powershell
pwsh -File scripts/test-template.ps1
```

Cakupan pengujian meliputi:
- **Domain Unit Tests**: Logika domain entity, value object, dan Result pattern.
- **Application Unit Tests**: Validasi FluentValidation dan CQRS Handler dengan NSubstitute mocking.
- **WebApi Integration Tests**: Pengujian endpoint menyeluruh menggunakan `WebApplicationFactory` dan SQLite/InMemory database terisolasi.

---

## 📄 Lisensi
Proyek template ini didistribusikan di bawah lisensi MIT.
