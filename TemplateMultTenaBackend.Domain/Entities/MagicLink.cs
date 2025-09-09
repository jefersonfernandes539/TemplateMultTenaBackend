namespace TemplateMultTenaBackend.Domain.Entities
{
    public class MagicLink : BaseModel
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime? UsedAt { get; set; }

        public bool IsExpired()
        {
            return DateTime.Compare(CreatedAt.AddDays(7), DateTime.UtcNow) < 0;
        }
    }
}
