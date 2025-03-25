

namespace EPSPlus.Application.DTOs;

public class BenefitEligibilityDto
{
    public string? EligibilityId { get; set; }
    public string? MemberID { get; set; }
    public DateTime EligibleDate { get; set; }
    public bool Status { get; set; } // "Eligible", "Not Eligible"
}

