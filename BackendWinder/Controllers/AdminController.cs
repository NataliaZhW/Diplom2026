using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendWinder.Data;
using BackendWinder.Models.Admin;
using BackendWinder.Models.Tasks;
using System.Security.Claims;

namespace BackendWinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]  // Только админы имеют доступ
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly ReferenceDbContext _referenceDbContext;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            AppDbContext appDbContext,
            ReferenceDbContext referenceDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _referenceDbContext = referenceDbContext;
        }

        // ================================================================
        // 1. ПОЛУЧИТЬ ВСЕХ ПОЛЬЗОВАТЕЛЕЙ
        // GET /api/admin/users
        // ================================================================
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserAdminDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var profile = await _appDbContext.UserProfiles
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                result.Add(new UserAdminDto
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    FullName = profile?.FullName ?? user.Email ?? "",
                    Roles = roles.ToList(),
                    IsActive = true,
                    CreatedAt = profile?.CreatedAt ?? DateTime.Now
                });
            }

            return Ok(result);
        }

        // ================================================================
        // 2. ОБНОВИТЬ РОЛЬ ПОЛЬЗОВАТЕЛЯ
        // POST /api/admin/users/role
        // ================================================================
        [HttpPost("users/role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            if (request.Add)
            {
                // Добавляем роль
                var result = await _userManager.AddToRoleAsync(user, request.Role);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Ошибка добавления роли", errors = result.Errors });
                }
            }
            else
            {
                // Удаляем роль
                var result = await _userManager.RemoveFromRoleAsync(user, request.Role);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Ошибка удаления роли", errors = result.Errors });
                }
            }

            return Ok(new { message = $"Роль '{request.Role}' успешно {(request.Add ? "добавлена" : "удалена")}" });
        }

        // ================================================================
        // 3. ПОЛУЧИТЬ ВСЕ ЗАДАНИЯ (ВСЕХ ПОЛЬЗОВАТЕЛЕЙ)
        // GET /api/admin/tasks
        // ================================================================
        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _appDbContext.AssignedTasks
                .OrderByDescending(t => t.AssignedAt)
                .ToListAsync();

            var taskDtos = tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                PlannedTaskId = t.PlannedTaskId,
                AssignedToUserId = t.AssignedToUserId,
                TaskType = t.TaskType,
                KitSchemeCode = t.KitSchemeCode,
                KitSchemeName = t.KitSchemeName,
                BrandCode = t.BrandCode,
                ColorCode = t.ColorCode,
                CountCode = t.CountCode,
                Quantity = t.Quantity,
                Notes = t.Notes,
                Status = t.Status,
                AssignedAt = t.AssignedAt,
                MaterialsRequestedAt = t.MaterialsRequestedAt,
                MaterialsReceivedAt = t.MaterialsReceivedAt,
                SubmittedAt = t.SubmittedAt,
                ReportedAt = t.ReportedAt,
                ArchivedAt = t.ArchivedAt
            }).ToList();

            var response = new AllTasksResponse
            {
                Tasks = taskDtos,
                TotalCount = taskDtos.Count,
                NewCount = taskDtos.Count(t => t.Status == "New"),
                InProgressCount = taskDtos.Count(t => t.Status == "MaterialsRequested" || t.Status == "MaterialsReceived" || t.Status == "InProgress"),
                SubmittedCount = taskDtos.Count(t => t.Status == "Submitted" || t.Status == "Reported"),
                ArchivedCount = taskDtos.Count(t => t.Status == "Archived")
            };

            return Ok(response);
        }

        // ================================================================
        // 4. ПОЛУЧИТЬ ЗАДАНИЯ КОНКРЕТНОГО ПОЛЬЗОВАТЕЛЯ
        // GET /api/admin/users/{userId}/tasks
        // ================================================================
        [HttpGet("users/{userId}/tasks")]
        public async Task<IActionResult> GetUserTasks(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            var tasks = await _appDbContext.AssignedTasks
                .Where(t => t.AssignedToUserId == userId)
                .OrderByDescending(t => t.AssignedAt)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    PlannedTaskId = t.PlannedTaskId,
                    AssignedToUserId = t.AssignedToUserId,
                    TaskType = t.TaskType,
                    KitSchemeCode = t.KitSchemeCode,
                    KitSchemeName = t.KitSchemeName,
                    BrandCode = t.BrandCode,
                    ColorCode = t.ColorCode,
                    CountCode = t.CountCode,
                    Quantity = t.Quantity,
                    Notes = t.Notes,
                    Status = t.Status,
                    AssignedAt = t.AssignedAt,
                    MaterialsRequestedAt = t.MaterialsRequestedAt,
                    MaterialsReceivedAt = t.MaterialsReceivedAt,
                    SubmittedAt = t.SubmittedAt,
                    ReportedAt = t.ReportedAt,
                    ArchivedAt = t.ArchivedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // ================================================================
        // 5. РЕДАКТИРОВАТЬ ЗАДАНИЕ (изменить количество и т.д.)
        // PUT /api/admin/tasks/{id}
        // ================================================================
        [HttpPut("tasks/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskQuantityRequest request)
        {
            var task = await _appDbContext.AssignedTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound(new { message = "Задание не найдено" });
            }

            task.Quantity = request.Quantity;
            task.Notes = request.Notes ?? task.Notes;

            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = "Задание обновлено" });
        }

        // ================================================================
        // 6. УДАЛИТЬ ЗАДАНИЕ
        // DELETE /api/admin/tasks/{id}
        // ================================================================
        [HttpDelete("tasks/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _appDbContext.AssignedTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound(new { message = "Задание не найдено" });
            }

            _appDbContext.AssignedTasks.Remove(task);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = "Задание удалено" });
        }
    }

    // Вспомогательный DTO для обновления задания
    public class UpdateTaskQuantityRequest
    {
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
}