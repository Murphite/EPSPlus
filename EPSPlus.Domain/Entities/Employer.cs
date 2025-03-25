

using System.ComponentModel.DataAnnotations;

namespace EPSPlus.Domain.Entities;

public class Employer : Entity
{
    public string? UserId { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
    public string? CompanyName { get; set; }

    [Required]
    public string? RegistrationNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // List of Members under the Employer
    public List<Member> Members { get; set; } = new List<Member>();

    // Navigation Property
    public ApplicationUser? User { get; set; }


    //// Business Rule: Employer must be active and have a valid registration number
    //public void ValidateEmployer()
    //{
    //    if (!ActiveStatus)
    //        throw new ValidationException("Employer must be active.");

    //    if (string.IsNullOrWhiteSpace(RegistrationNumber))
    //        throw new ValidationException("Employer must have a valid registration number.");
    //}
}
