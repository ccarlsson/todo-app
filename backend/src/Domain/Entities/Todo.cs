using TodoApp.Domain.ValueObjects;

namespace TodoApp.Domain.Entities;

public sealed class Todo
{
    public string Id { get; init; }
    public string UserId { get; init; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Priority? Priority { get; private set; }
    public TodoStatus Status { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }

    public Todo(
        string userId,
        string title,
        string? description,
        DateTime? dueDate,
        Priority? priority)
    {
        Id = Guid.NewGuid().ToString("N");
        UserId = userId;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        Status = TodoStatus.NotStarted;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Update(
        string? title,
        string? description,
        DateTime? dueDate,
        Priority? priority,
        TodoStatus? status)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
        }

        if (description is not null)
        {
            Description = description;
        }

        if (dueDate.HasValue)
        {
            DueDate = dueDate;
        }

        if (priority.HasValue)
        {
            Priority = priority;
        }

        if (status.HasValue)
        {
            Status = status.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
