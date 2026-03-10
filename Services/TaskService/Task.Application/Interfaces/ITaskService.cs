using System.Threading.Tasks;
using TaskService.Application.DTOs;

namespace TaskService.Application.Interfaces;

public interface ITaskService
{
    Task CreateTaskAsync(CreateTaskRequest request, Guid userId);

    Task<List<TaskResponse>> GetUserTasksAsync(Guid userId, string role);

    Task DeleteTaskAsync(Guid taskId, Guid userId, string role);
}