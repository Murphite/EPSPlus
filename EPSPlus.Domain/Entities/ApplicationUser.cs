

using EPSPlus.Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace EPSPlus.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public new string? PhoneNumber { get; set; }
    public UserType UserType { get; set; } // "Member" or "Employer"
    public DateTime CreatedAt { get; set; }

    // Navigation properties 
    public Member? MemberDetails { get; set; }
    public Employer? EmployerDetails { get; set; }
}
