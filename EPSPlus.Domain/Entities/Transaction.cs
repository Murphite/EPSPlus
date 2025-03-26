

using EPSPlus.Domain.Enum;

namespace EPSPlus.Domain.Entities;

public class Transaction : Entity
{
    public string? ContributionId { get; set; }
    public TransactionStatus? Status { get; set; } // "Success", "Failed", "Pending"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Contribution? Contribution { get; set; }

}
