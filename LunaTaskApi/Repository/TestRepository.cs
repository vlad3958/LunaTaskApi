using LunaTaskApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LunaTaskApi.Repository;
public class TaskRepository : ITaskRepository // наслідуємо інтерфейс
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Task>> GetTasksAsync(Guid userId, TaskFilter filter)
    {
        IQueryable<Task> tasks = _context.Tasks.Where(t => t.UserId == userId);

        // Застосування фільтрів
        if (filter.Status.HasValue)
        {
            tasks = tasks.Where(t => t.Status == filter.Status.Value);
        }

        if (filter.Priority.HasValue)
        {
            tasks = tasks.Where(t => t.Priority == filter.Priority.Value);
        }

        if (filter.DueDate.HasValue)
        {
            tasks = tasks.Where(t => t.DueDate <= filter.DueDate.Value);
        }

        // Пагінація
        if (filter.Page > 0 && filter.PageSize > 0)
        {
            tasks = tasks.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);
        }

        return await tasks.ToListAsync();
    }

    public async Task<Task> GetTaskByIdAsync(Guid id, Guid userId)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async System.Threading.Tasks.Task AddTaskAsync(Task task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }


    public async Task<bool> UpdateTaskAsync(Guid taskId, Guid userId, UpdateTaskDto updateTaskDto)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task == null) return false;

        // Оновлення полів таски
        task.Title = updateTaskDto.Title ?? task.Title;
        task.Description = updateTaskDto.Description ?? task.Description;
        task.DueDate = updateTaskDto.DueDate ?? task.DueDate;
        task.Status = updateTaskDto.Status ?? task.Status;
        task.Priority = updateTaskDto.Priority ?? task.Priority;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        return true;
    }

    // Видалити таску
    public async Task<bool> DeleteTaskAsync(Guid taskId, Guid userId)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }
}
