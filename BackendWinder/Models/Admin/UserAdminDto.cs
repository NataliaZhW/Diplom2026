namespace BackendWinder.Models.Admin
{
    // DTO для передачи данных о пользователе администратору
    public class UserAdminDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}