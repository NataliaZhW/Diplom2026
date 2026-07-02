using BackendWinder.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendWinder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly OutsideContext _context;

    public TestController(OutsideContext context)
    {
        _context = context;
    }

    [HttpGet("check-password")]
    public async Task<IActionResult> CheckPassword(string login, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == login);

        if (user == null)
        {
            return Ok(new { message = $"Пользователь {login} не найден" });
        }

        var hashFromDb = user.PasswordHash;
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, hashFromDb);

        // Генерируем тестовый хеш для сравнения
        var testHash = BCrypt.Net.BCrypt.HashPassword("password123");

        return Ok(new
        {
            login = login,
            userFound = true,
            passwordFromRequest = password,
            hashFromDb = hashFromDb,
            hashLength = hashFromDb?.Length ?? 0,
            isPasswordValid = isPasswordValid,
            testHash = testHash,
            testHashLength = testHash?.Length ?? 0,
            note = "Используйте этот хеш для обновления БД"
        });
    }
}