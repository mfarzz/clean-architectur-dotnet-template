# Project Plan: .NET Web API Clean Architecture Starter Template (`MyCleanApi`)

## 1. Ringkasan Proyek
Proyek ini bertujuan untuk membangun *starter repository* (template dasar) .NET Web API berbasis **Clean Architecture** yang siap pakai, mudah dipelihara, dan dapat digunakan ulang untuk berbagai proyek baru melalui fitur **GitHub Template Repository** serta **Custom `dotnet new` Template**.

### Tujuan Utama
- Mempercepat *setup* awal proyek .NET Web API baru tanpa konfigurasi berulang.
- Menegakkan pemisahan tanggung jawab (*Separation of Concerns*) dan aturan *dependency* secara ketat antar layer.
- Menyediakan pondasi standar industri (CQRS, validasi otomatis, *global error handling*, *structured logging*, dan Docker) tanpa *over-engineering*.
- Memungkinkan penggantian nama proyek dan *namespace* secara otomatis melalui .NET CLI.

---

## 2. Arsitektur & Tech Stack

### Arsitektur (4-Layer Clean Architecture)
1. **`MyCleanApi.Domain`**: Inti bisnis murni (Entities, Value Objects, Enums, Domain Exceptions, Result Pattern). Nol dependensi eksternal.
2. **`MyCleanApi.Application`**: Logika aplikasi/use cases menggunakan pola CQRS dengan Feature Folders, DTOs, Validation Pipeline, dan *interfaces*.
3. **`MyCleanApi.Infrastructure`**: Implementasi EF Core, *DbContext*, *Entity Configurations*, *Interceptors*, Repository, dan layanan eksternal.
4. **`MyCleanApi.WebApi`**: Entry point aplikasi (Controllers / Minimal APIs), Middleware, Global Exception Handling, DI Registration, dan dokumentasi API.

### Core Tech Stack
- **Framework**: .NET 10 (`net10.0`) Web API
- **ORM & Database**: Entity Framework Core dengan *Multi-Provider Support* (PostgreSQL & SQL Server) yang dapat dipilih via konfigurasi / parameter template
- **API Style**: Mendukung pilihan **Controllers** atau **Minimal APIs** via parameter template (`--api-style`)
- **CQRS & Mediator**: MediatR
- **Validation**: FluentValidation
- **Logging**: Serilog (`Serilog.AspNetCore`)
- **API Documentation**: OpenAPI / Scalar & Swagger
- **Testing**: xUnit, NSubstitute, FluentAssertions, WebApplicationFactory
- **Package Management**: Central Package Management (`Directory.Packages.props`)
- **Containerization**: Docker & Docker Compose (layanan PostgreSQL dan SQL Server lokal)

---

## 3. Roadmap & Checklist Pengerjaan

### Fase 1: Inisialisasi Solution & Struktur Dasar
- [x] Inisialisasi Git repository dan tambahkan `.gitignore` standar .NET.
- [x] Buat solution `MyCleanApi.slnx` dan 4 project utama di dalam folder `src/`.
- [x] Buat kerangka project test di dalam folder `tests/` (`Domain.UnitTests`, `Application.UnitTests`, `WebApi.IntegrationTests`).
- [x] Konfigurasi *Project References* antar layer sesuai aturan *Dependency Inversion*.
- [x] Setup **Central Package Management** menggunakan file `Directory.Packages.props`.
- [x] Tambahkan `.editorconfig` untuk standar format kode yang konsisten.

### Fase 2: Pondasi Domain & Application Layer
- [x] **Domain**: Buat `BaseEntity` dan `BaseAuditableEntity` (`Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`).
- [x] **Domain**: Implementasikan **Result Pattern** (`Result`, `Result<T>`, `Error`).
- [x] **Application**: Buat *interfaces* inti (`IApplicationDbContext`, `IDateTimeProvider`, `ICurrentUserService`).
- [x] **Application**: Buat model pendukung (`PaginatedList<T>`, `PaginationParams`).
- [x] **Application**: Implementasikan MediatR `IPipelineBehavior` untuk validasi otomatis (`ValidationBehavior`) dan logging (`LoggingBehavior`).
- [x] **Application**: Buat `DependencyInjection.cs` untuk registrasi MediatR dan FluentValidation.
- [x] **Application**: Buat 1 fitur contoh (*Sample Feature*, misal `SampleItems`) lengkap dengan Command, Query, Validator, Handler, dan DTO sebagai referensi pola kode.

### Fase 3: Infrastructure & Persistence Layer
- [x] Setup `ApplicationDbContext` beserta mekanisme *auto-apply* `IEntityTypeConfiguration<T>`.
- [x] Implementasikan dynamic/switchable DbContext options: support PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`), SQL Server (`Microsoft.EntityFrameworkCore.SqlServer`), dan InMemory melalui konfigurasi `"DatabaseProvider"`.
- [x] Buat `AuditableEntityInterceptor` untuk mengisi timestamp audit (`CreatedAt`/`UpdatedAt`) secara otomatis saat `SaveChangesAsync`.
- [x] Konfigurasi entity untuk *Sample Feature* dan siapkan initial migrations/seeding.
- [x] Buat `DependencyInjection.cs` untuk registrasi `DbContext`, *Interceptors*, dan *Services*.
- [x] Buat `docker-compose.yml` dengan profile untuk PostgreSQL dan SQL Server lokal.

### Fase 4: WebApi & Cross-Cutting Concerns
- [x] Implementasikan `GlobalExceptionHandler` menggunakan `IExceptionHandler` yang mengembalikan format standar **RFC 7807 (`ProblemDetails`)**.
- [x] Setup **Serilog** (konfigurasi di `appsettings.json`, output ke console dan rolling file).
- [x] Konfigurasi dokumentasi OpenAPI (Scalar / Swagger).
- [x] Sediakan implementasi **Controllers** (`SampleItemsController`) dan **Minimal APIs** (`SampleItemEndpoints`) dengan abstraction `EndpointResultExtensions` / `ApiControllerBase`.
- [x] Tambahkan Health Checks endpoint (`/health`).
- [x] Buat `Dockerfile` multi-stage build yang optimal untuk production.

### Fase 5: Testing & Verifikasi Kualitas
- [x] Buat contoh Unit Test untuk Domain Entity / Value Object.
- [x] Buat contoh Unit Test untuk Application Command Handler dan Validator.
- [x] Setup kerangka Integration Test (menggunakan `WebApplicationFactory`).
- [x] Pastikan seluruh project dapat di-build tanpa warning kritis dan semua test lulus (`dotnet test`).

### Fase 6: Packaging sebagai Custom Template & Dokumentasi
- [x] Buat folder `.template.config/template.json` dengan:
  - `sourceName: "MyCleanApi"`
  - `shortName: "cleanapi"`
  - Simbol pilihan Database: `-D, --Database` (`postgres` | `sqlserver`)
  - Simbol pilihan Web API: `-A, --ApiStyle` (`controllers` | `minimal`)
- [x] Uji instalasi lokal (`dotnet new install .`) dan uji generasi proyek baru dengan kombinasi parameter (`dotnet new cleanapi -n SampleGeneratedApi`).
- [x] Verifikasi proyek hasil generate dapat langsung di-build dan dijalankan dengan 100% test lulus.
- [x] Tulis `README.md` yang lengkap (arsitektur, cara menjalankan via Docker, cara memakai sebagai `dotnet new` template, dan panduan konfigurasi).

---

## 4. Kriteria Selesai (Definition of Done - v1.0)
1. Menjalankan `dotnet new cleanapi -n NamaProyekBaru` menghasilkan solution baru yang bersih, *buildable*, dan langsung siap dijalankan.
2. Seluruh aturan dependensi layer terjaga (Domain tidak bergantung pada package eksternal apa pun).
3. Request tidak valid otomatis tertangkap oleh `ValidationBehavior` dan mengembalikan respons `400 Bad Request` dalam format `ProblemDetails`.
4. Error tidak terduga tertangkap oleh `GlobalExceptionHandler` dan tercatat di log Serilog.
5. `docker-compose up` dapat menyalakan database lokal dan API dapat terhubung dengan lancar.

---

## 5. Ide Pengembangan Lanjutan (v2.0 / Opsional)
- Parameter pilihan database pada `template.json` (`--db sqlserver | postgres`).
- Opsional *flag* untuk menyertakan boilerplate Authentication & Authorization (JWT + ASP.NET Core Identity).
- Integrasi GitHub Actions workflow (CI) untuk otomatis menjalankan `dotnet build` dan `dotnet test` pada setiap *Pull Request*.