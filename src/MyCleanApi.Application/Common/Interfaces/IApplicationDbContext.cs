using Microsoft.EntityFrameworkCore;
using MyCleanApi.Domain.Entities;

namespace MyCleanApi.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<SampleItem> SampleItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
