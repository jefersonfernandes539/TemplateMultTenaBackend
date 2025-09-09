using System.ComponentModel.DataAnnotations;
using TemplateMultTenaBackend.Application.Attributes;

namespace TemplateMultTenaBackend.Domain.Entities
{
    public class UserRefreshToken
    {
        [Key]
        [Required, NotEmpty]
        public string Token { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedByIp { get; set; } = null!;
        public string? CreatedByUserAgent { get; set; } = null!;

        public DateTime? RevokedAt { get; set; } = null!;
        public string? RevokedByIp { get; set; }
        public string? RevokedByUserAgent { get; set; }

        [Required, NotEmpty]
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public static int ValidForDays => 7;
        public bool IsExpired => (DateTime.UtcNow >= CreatedAt.AddDays(ValidForDays)) || (RevokedAt is not null);
        public bool IsValid => !IsExpired;
    }
}
