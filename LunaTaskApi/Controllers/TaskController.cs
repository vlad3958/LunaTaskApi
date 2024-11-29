using LunaTaskApi;
using LunaTaskApi.Repository; // Підключаємо репозиторій
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LunaTaskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        // Конструктор приймає залежність через інтерфейс репозиторію
        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // Отримати всі таски для юзера
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilter filter)
        {
            var userId = GetUserIdFromClaims(); // Отримуємо userId з JWT

            // Отримуємо відфільтровані таски через репозиторій
            var tasks = await _taskRepository.GetTasksAsync(userId, filter);

            return Ok(tasks); // Повертаємо список тасок
        }

        // Отримати таску по айді
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var userId = GetUserIdFromClaims();

            // Шукаємо таску через репозиторій
            var task = await _taskRepository.GetTaskByIdAsync(id, userId);

            if (task == null)
            {
                return NotFound(); // Якщо не знайдено
            }

            return Ok(task); // Повертаємо знайдену таску
        }

        // Створити нову таску
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var userId = GetUserIdFromClaims();

            var task = new Task
            {
                Id = Guid.NewGuid(),
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                Status = TaskStatus.Pending, // Статус за замовчуванням
                Priority = createTaskDto.Priority,
                UserId = userId
            };

            // Додаємо таску через репозиторій
            await _taskRepository.AddTaskAsync(task);

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task); // Повертаємо посилання на нову таску
        }

        // Оновити існуючу таску
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var userId = GetUserIdFromClaims();

            // Оновлюємо таску через репозиторій
            var success = await _taskRepository.UpdateTaskAsync(id, userId, updateTaskDto);

            if (!success)
            {
                return NotFound(); // Якщо таска не знайдена або не належить юзеру
            }

            return NoContent(); // Успішно оновлено
        }

        // Видалити таску
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetUserIdFromClaims();

            // Видаляємо таску через репозиторій
            var success = await _taskRepository.DeleteTaskAsync(id, userId);

            if (!success)
            {
                return NotFound(); // Якщо таска не знайдена або не належить юзеру
            }

            return NoContent(); // Успішно видалено
        }

        // Допоміжний метод щоб отримувати userId із JWT
        private Guid GetUserIdFromClaims()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User not authorized. Claim 'nameidentifier' not found.");
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User not authorized. Invalid userId in claim.");
            }

            return userId;
        }
    }
}
