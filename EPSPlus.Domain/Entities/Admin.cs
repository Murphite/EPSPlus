

namespace EPSPlus.Domain.Entities;

public class Admin : Entity
{
    public DateTimeOffset CreatedAt { get; set; }
    public string UserId { get; set; } = default!;
    public ApplicationUser? User { get; set; }
}
