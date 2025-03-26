

using EPSPlus.Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace EPSPlus.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public new string? PhoneNumber { get; set; }
    public string UserType { get; set; } // "Member" or "Employer" or Admin
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties 
    public Member? MemberDetails { get; set; }
    public Employer? EmployerDetails { get; set; }
    public Admin? AdminDetails { get; set; }
}
