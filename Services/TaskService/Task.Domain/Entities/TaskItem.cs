namespace TaskService.Domain.Entities;
public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; } = false;

    public Guid UserId { get; set; }  // Owner of task

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}