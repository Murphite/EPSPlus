

using System.ComponentModel.DataAnnotations;

namespace EPSPlus.Application.DTOs;

public class MemberDto
{
    public string? Id { get; set; }
    public string? FullName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string CompanyName { get; set; } = string.Empty;

    [Range(18, 70, ErrorMessage = "Member must be between 18 and 70 years old.")]
    public int Age { get; set; }
    public bool ActiveStatus { get; set; }
}

public class UpdateMemberDto
{
    public string? Id { get; set; }
    public string? FullName { get; set; }
    public bool IsActive { get; set; }
    public string Email { get; set; } = string.Empty;
    public string EmployerId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}


