using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarAgent_BE.Data
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? CreatedByIp { get; set; }

        public DateTime? RevokedAtUtc { get; set; }

        public string? RevokedByIp { get; set; }

        public string? ReplacedByToken { get; set; }

        // Conveniență
        public bool IsActive => RevokedAtUtc == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    }
}
