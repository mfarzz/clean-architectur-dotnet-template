using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCleanApi.Application.Common.Interfaces;
using MyCleanApi.Infrastructure.Persistence;
using MyCleanApi.Infrastructure.Persistence.Interceptors;
using MyCleanApi.Infrastructure.Services;

namespace MyCleanApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<AuditableEntityInterceptor>();

        var provider = configuration["DatabaseProvider"] ?? "PostgreSQL";
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=MyCleanDb;Username=postgres;Password=postgres;";

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            options.AddInterceptors(interceptor);

            if (provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase("MyCleanDb_InMemory");
            }
            else if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseNpgsql(connectionString);
            }
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
