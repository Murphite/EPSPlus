

using System.ComponentModel.DataAnnotations;

namespace EPSPlus.Application.DTOs;

public class EmployerDto
{
    public string? Id { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public string RegistrationNumber { get; set; } = string.Empty;

    public bool ActiveStatus { get; set; }
    public List<MemberDto>? Members { get; set; }
}


