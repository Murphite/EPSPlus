

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
    public List<EmployerMembersDto>? Members { get; set; }
}

public class UpdateEmployerDto
{
    public string? Id { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public bool ActiveStatus { get; set; }
}



public class EmployerMembersDto
{
    public string? Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public bool ActiveStatus { get; set; }
}

