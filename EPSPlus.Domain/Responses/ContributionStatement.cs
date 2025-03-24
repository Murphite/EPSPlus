
using EPSPlus.Domain.Entities;

namespace EPSPlus.Domain.Responses;

public class ContributionStatement
{
    public string? MemberId { get; set; }
    public decimal TotalContributions { get; set; }
    public List<Contribution> Contributions { get; set; } = new List<Contribution>();
}
