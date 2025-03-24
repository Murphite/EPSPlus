

namespace EPSPlus.Domain.Entities;

public class BenefitEligibility : Entity
{
    public string? MemberId { get; set; }
    public DateTime EligibleDate { get; set; }
    public bool Status { get; set; }  // "Eligible", "Not Eligible"

    // Navigation Property
    public Member? Member { get; set; }
}
