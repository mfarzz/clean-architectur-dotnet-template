using MyCleanApi.Application.Common.Interfaces;

namespace MyCleanApi.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
