using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyCleanApi.Application.Common.Interfaces;
using MyCleanApi.Domain.Entities;

namespace MyCleanApi.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SampleItem> SampleItems => Set<SampleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
