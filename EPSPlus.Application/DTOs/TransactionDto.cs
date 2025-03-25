

using System.Transactions;

namespace EPSPlus.Application.DTOs;

public class TransactionDto
{
    public string? TransactionId { get; set; }
    public string? ContributionId { get; set; }
    public TransactionStatus Status { get; set; } // "Success", "Failed", "Pending"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
