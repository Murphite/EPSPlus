

using EPSPlus.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace EPSPlus.Domain.Entities;

public class Contribution : Entity
{
    [Required]
    public string? MemberId { get; set; }

    [Required]
    public ContributionStatus ContributionType { get; set; } // "Monthly" or "Voluntary"

    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Contribution amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime ContributionDate { get; set; }

    public string? Status { get; set; }

    // Navigation Property
    public Member? Member { get; set; }
    public ICollection<Transaction>? Transactions { get; set; }
}

// Business Rule: Contribution Date must be valid (not in the future)
//public void ValidateContributionDate()
//{
//    if (ContributionDate > DateTime.UtcNow)
//    {
//        throw new ValidationException("Contribution date cannot be in the future.");
//    }
//}