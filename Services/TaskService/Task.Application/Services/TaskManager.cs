using TaskService.Application.DTOs;
using TaskService.Application.Interfaces;
using TaskService.Domain.Entities;
using TaskService.Domain.Interfaces;

namespace TaskService.Application.Services;

public class TaskManager : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskManager(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateTaskAsync(CreateTaskRequest request, Guid userId)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            UserId = userId
        };

        await _repository.AddAsync(task);
        await _repository.SaveChangesAsync();
    }

    public async Task<List<TaskResponse>> GetUserTasksAsync(Guid userId, string role)
    {
        var tasks = await _repository.GetUserTasksAsync(userId);

        return tasks.Select(t => new TaskResponse
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted
        }).ToList();
    }

    public async Task DeleteTaskAsync(Guid taskId, Guid userId, string role)
    {
        var task = await _repository.GetByIdAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        if (task.UserId != userId && role != "Admin")
            throw new Exception("Unauthorized access");

        await _repository.DeleteAsync(task);
        await _repository.SaveChangesAsync();
    }
}