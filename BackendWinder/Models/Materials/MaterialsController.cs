using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using BackendWinder.Data;
using BackendWinder.Models.Materials;
using System.Security.Claims;

namespace BackendWinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Все эндпоинты требуют авторизации
    public class MaterialsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public MaterialsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // Получить ID текущего пользователя из токена
        private string GetCurrentUserId()
        {
            return User.FindFirst("userId")?.Value ?? throw new Exception("User not found");
        }

        // ================================================================
        // 1. ПОЛУЧИТЬ ВСЕ ЗАПРОСЫ ТЕКУЩЕГО ПОЛЬЗОВАТЕЛЯ
        // GET /api/materials/requests/my
        // ================================================================
        [HttpGet("requests/my")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = GetCurrentUserId();
            
            var requests = await _appDbContext.MaterialRequests
                .Where(r => r.RequestedByUserId == userId)
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new MaterialRequestDto
                {
                    Id = r.Id,
                    TaskIds = r.TaskIds,
                    RequestedByUserId = r.RequestedByUserId,
                    Materials = r.Materials,
                    Status = r.Status,
                    RequestedAt = r.RequestedAt,
                    ReceivedAt = r.ReceivedAt,
                    Notes = r.Notes
                })
                .ToListAsync();

            return Ok(requests);
        }

        // ================================================================
        // 2. ПОЛУЧИТЬ ЗАПРОС ПО ID
        // GET /api/materials/requests/{id}
        // ================================================================
        [HttpGet("requests/{id}")]
        public async Task<IActionResult> GetRequest(int id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            
            var request = await _appDbContext.MaterialRequests
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound(new { message = "Запрос не найден" });

            // Проверка прав: только владелец или админ
            if (request.RequestedByUserId != userId && !isAdmin)
                return Forbid();

            return Ok(request);
        }

        // ================================================================
        // 3. СОЗДАТЬ НОВЫЙ ЗАПРОС МАТЕРИАЛОВ
        // POST /api/materials/requests
        // ================================================================
        [HttpPost("requests")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateMaterialRequest request)
        {
            var userId = GetCurrentUserId();
            
            // Проверяем, что все задания существуют и принадлежат пользователю
            foreach (var taskId in request.TaskIds)
            {
                var task = await _appDbContext.AssignedTasks
                    .FirstOrDefaultAsync(t => t.Id == taskId);
                    
                if (task == null)
                    return BadRequest(new { message = $"Задание {taskId} не найдено" });
                    
                if (task.AssignedToUserId != userId)
                    return Forbid();
            }

            // Создаем запрос материалов
            var materialRequest = new MaterialRequest
            {
                TaskIds = JsonSerializer.Serialize(request.TaskIds),
                RequestedByUserId = userId,
                Materials = request.Materials,
                Status = "Pending",
                RequestedAt = DateTime.Now,
                Notes = request.Notes
            };

            _appDbContext.MaterialRequests.Add(materialRequest);
            
            // Обновляем статусы заданий
            foreach (var taskId in request.TaskIds)
            {
                var task = await _appDbContext.AssignedTasks
                    .FirstOrDefaultAsync(t => t.Id == taskId);
                if (task != null && task.Status == "New")
                {
                    task.Status = "MaterialsRequested";
                    task.MaterialsRequestedAt = DateTime.Now;
                }
            }
            
            await _appDbContext.SaveChangesAsync();

            return Ok(new { id = materialRequest.Id, message = "Запрос материалов создан" });
        }

        // ================================================================
        // 4. ПОДТВЕРДИТЬ ПОЛУЧЕНИЕ МАТЕРИАЛОВ (для мотальщика)
        // POST /api/materials/requests/receive
        // ================================================================
        [HttpPost("requests/receive")]
        public async Task<IActionResult> ReceiveMaterials([FromBody] ReceiveMaterialRequest request)
        {
            var userId = GetCurrentUserId();
            
            var materialRequest = await _appDbContext.MaterialRequests
                .FirstOrDefaultAsync(r => r.Id == request.RequestId);

            if (materialRequest == null)
                return NotFound(new { message = "Запрос не найден" });

            if (materialRequest.RequestedByUserId != userId)
                return Forbid();

            if (materialRequest.Status != "Pending")
                return BadRequest(new { message = "Запрос уже обработан" });

            // Обновляем статус запроса
            materialRequest.Status = "Received";
            materialRequest.ReceivedAt = DateTime.Now;

            // Обновляем статусы заданий
            var taskIds = JsonSerializer.Deserialize<List<int>>(materialRequest.TaskIds);
            if (taskIds != null)
            {
                foreach (var taskId in taskIds)
                {
                    var task = await _appDbContext.AssignedTasks
                        .FirstOrDefaultAsync(t => t.Id == taskId);
                    if (task != null && task.Status == "MaterialsRequested")
                    {
                        task.Status = "MaterialsReceived";
                        task.MaterialsReceivedAt = DateTime.Now;
                    }
                }
            }

            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = "Получение материалов подтверждено" });
        }

        // ================================================================
        // 5. ОТМЕНИТЬ ЗАПРОС (для админа)
        // DELETE /api/materials/requests/{id}
        // ================================================================
        [HttpDelete("requests/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var request = await _appDbContext.MaterialRequests
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound(new { message = "Запрос не найден" });

            request.Status = "Cancelled";
            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = "Запрос отменен" });
        }
    }
}