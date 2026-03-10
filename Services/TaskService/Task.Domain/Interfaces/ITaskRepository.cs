using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Domain.Entities;

namespace TaskService.Domain.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);

    Task<List<TaskItem>> GetUserTasksAsync(Guid userId);

    Task<TaskItem?> GetByIdAsync(Guid id);

    Task DeleteAsync(TaskItem task);

    Task SaveChangesAsync();
}