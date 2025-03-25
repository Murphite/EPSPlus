

namespace EPSPlus.Domain.Entities;

public class Admin : Entity
{
    public DateTimeOffset CreatedAt { get; set; }
    public string ApplicationUserId { get; set; } = default!;
    public ApplicationUser? ApplicationUser { get; set; }
}
