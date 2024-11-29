using LunaTaskApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace LunaTaskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        // Отримати всі таски для юзера
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilter filter)
        {
            var userId = GetUserIdFromClaims();

            IQueryable<Task> tasks = _context.Tasks.Where(t => t.UserId == userId);

            // Застосовуємо фільтри
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

            // Pagination
            if (filter.Page > 0 && filter.PageSize > 0)
            {
                tasks = tasks.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);
            }

            var result = await tasks.ToListAsync();

            return Ok(result);
        }

        // Отримати таску по айді
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var userId = GetUserIdFromClaims();

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        // Створити нову таску
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var userId = GetUserIdFromClaims(); // отримуємо юзер айді по токену

            var task = new Task
            {
                Id = Guid.NewGuid(),
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                Status = TaskStatus.Pending, // Default status
                Priority = createTaskDto.Priority,
                UserId = userId
            };
            //додаємо до БД
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // Оновити існуючу таску
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var userId = GetUserIdFromClaims(); // отримуємо юзер айді по токену

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
            {
                return NotFound();
            }

            // перевірка на null, чи є в нас зміни
            task.Title = updateTaskDto.Title ?? task.Title;
            task.Description = updateTaskDto.Description ?? task.Description;
            task.DueDate = updateTaskDto.DueDate ?? task.DueDate;
            task.Status = updateTaskDto.Status ?? task.Status;
            task.Priority = updateTaskDto.Priority ?? task.Priority;
            task.UpdatedAt = DateTime.UtcNow;

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Видалити таску
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetUserIdFromClaims();

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
            {
                return NotFound();
            }
            // видаляємо із БД
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Допоміжний метод щоб отримувати userId із JWT
        private Guid GetUserIdFromClaims()
        {
            // Look for the 'nameidentifier' claim instead of 'sub'
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User not authorized. Claim 'sub' or 'nameidentifier' not found.");
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User not authorized. Invalid userId in claim.");
            }

            return userId;
        }



    }

    
}
