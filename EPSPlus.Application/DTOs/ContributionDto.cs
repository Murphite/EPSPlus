
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace EPSPlus.Application.DTOs;

public class ContributionDto
{
    [Required]
    public string MemberId { get; set; } = string.Empty;

    [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime ContributionDate { get; set; }
    public string? ContributionType { get; set; }
}

public class GetContributionDto
{
    [Required]
    public string MemberId { get; set; } = string.Empty;

    [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime ContributionDate { get; set; }
    public string? ContributionType { get; set; }
    public string? Status { get; set; }
}

public class ContributionStatementDto
{
    public string? MemberId { get; set; }
    public decimal TotalContributions { get; set; }
    public string? ContributionType { get; set; }
    public List<ContributionDto>? Contributions { get; set; }  
}

