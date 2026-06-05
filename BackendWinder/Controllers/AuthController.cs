using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendWinder.Data;
using BackendWinder.Models.Auth;
using BackendWinder.Services;

namespace BackendWinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _appDbContext;
        private readonly JwtService _jwtService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext appDbContext,
            JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appDbContext = appDbContext;
            _jwtService = jwtService;
        }

        // ================================================================
        // 1. РЕГИСТРАЦИЯ НОВОГО ПОЛЬЗОВАТЕЛЯ
        // ================================================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Проверяем, существует ли пользователь с таким email
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Пользователь с таким email уже существует" });
            }

            // Создаем нового пользователя Identity
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true  // Для упрощения, подтверждение не требуется
            };

            // Создаем пользователя в Identity
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // Возвращаем ошибки создания пользователя
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Ошибка регистрации", errors });
            }

            // Добавляем пользователю роль "User" по умолчанию
            await _userManager.AddToRoleAsync(user, "User");

            // Создаем профиль пользователя (дополнительная информация)
            var userProfile = new UserProfile
            {
                UserId = user.Id,
                FullName = request.FullName
            };
            _appDbContext.UserProfiles.Add(userProfile);
            await _appDbContext.SaveChangesAsync();

            // Генерируем JWT токен
            var roles = new List<string> { "User" };
            var token = _jwtService.GenerateToken(user.Id, user.Email, roles);

            // Возвращаем ответ
            return Ok(new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FullName = request.FullName,
                Roles = roles
            });
        }

        // ================================================================
        // 2. ВХОД В СИСТЕМУ (ЛОГИН)
        // ================================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Ищем пользователя по email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Неверный email или пароль" });
            }

            // Проверяем пароль
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Неверный email или пароль" });
            }

            // Получаем роли пользователя
            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = roles.ToList();

            // Получаем профиль пользователя (ФИО)
            var profile = await _appDbContext.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);
            var fullName = profile?.FullName ?? user.Email ?? "";

            // Генерируем JWT токен
            var email = user.Email ?? "";
            var token = _jwtService.GenerateToken(user.Id, email, rolesList);

            // Возвращаем ответ
            return Ok(new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                Email = email,
                FullName = fullName,
                Roles = rolesList
            });
        }

        // ================================================================
        // 3. ПОЛУЧЕНИЕ ПРОФИЛЯ ТЕКУЩЕГО ПОЛЬЗОВАТЕЛЯ
        // ================================================================
        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetProfile()
        {
            // Получаем ID пользователя из JWT токена
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Не удалось определить пользователя" });
            }

            // Находим пользователя в Identity
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            // Получаем роли
            var roles = await _userManager.GetRolesAsync(user);

            // Получаем профиль (ФИО)
            var profile = await _appDbContext.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            var fullName = profile?.FullName ?? user.Email ?? "";

            // Возвращаем ответ
            return Ok(new UserProfileResponse
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                FullName = fullName,
                Roles = roles.ToList()
            });
        }

        // ================================================================
        // 4. СМЕНА ПАРОЛЯ
        // ================================================================
        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Получаем ID пользователя из токена
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Не удалось определить пользователя" });
            }

            // Находим пользователя
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            // Смена пароля
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Ошибка смены пароля", errors });
            }

            return Ok(new { message = "Пароль успешно изменен" });
        }
    }
}