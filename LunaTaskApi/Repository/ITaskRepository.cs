
namespace LunaTaskApi.Repository
{
    // інтерфейс тасків
    public interface ITaskRepository
    {
        System.Threading.Tasks.Task<IEnumerable<Task>> GetTasksAsync(Guid userId, TaskFilter filter); // Отримати всі таски з фільтром
        System.Threading.Tasks.Task<Task> GetTaskByIdAsync(Guid id, Guid userId);
        System.Threading.Tasks.Task AddTaskAsync(Task task);
        System.Threading.Tasks.Task<bool> UpdateTaskAsync(Guid taskId, Guid userId, UpdateTaskDto updateTaskDto); // Оновити таску
        System.Threading.Tasks.Task<bool> DeleteTaskAsync(Guid taskId, Guid userId); // Видалити таску
    }

}
