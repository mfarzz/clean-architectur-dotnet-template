using MyCleanApi.Domain.Common;

namespace MyCleanApi.Domain.Entities;

public class SampleItem : BaseAuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsCompleted { get; private set; }

    private SampleItem() { }

    public SampleItem(string title, string? description)
    {
        Title = title;
        Description = description;
        IsCompleted = false;
    }

    public void Update(string title, string? description, bool isCompleted)
    {
        Title = title;
        Description = description;
        IsCompleted = isCompleted;
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
    }
}
