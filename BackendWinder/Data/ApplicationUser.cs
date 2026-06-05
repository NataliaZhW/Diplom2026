using Microsoft.AspNetCore.Identity;

namespace BackendWinder.Data
{
    // Этот класс расширяет стандартного пользователя Identity
    // Добавляет дополнительные поля в таблицу AspNetUsers
    public class ApplicationUser : IdentityUser
    {
        // Здесь можно добавить дополнительные поля, если нужно
        // Например, телефон, должность и т.д.
        // Пока оставляем пустым, используем отдельную таблицу UserProfiles
    }
}