namespace CarAgent_BE.Models
{
    public class UserLite
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // hash, nu plain
        public string? FullName { get; set; }
        public bool IsExternal { get; set; }
        public List<string> Roles { get; set; } = new() { "User" };
    }
}
