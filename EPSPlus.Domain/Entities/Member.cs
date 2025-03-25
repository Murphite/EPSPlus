using EPSPlus.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace EPSPlus.Domain.Entities;

public class Member : Entity
{
    public string? UserId { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    public int Age { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key to Employer
    public string? EmployerId { get; set; }
    public Employer? Employer { get; set; }


    // Navigation Property
    public ApplicationUser? User { get; set; }




    // Business Rule: Age must be between 18 and 70
    private int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;

        if (age < 18 || age > 70)
            throw new ValidationException("Member's age must be between 18 and 70 years.");

        return age;
    }
}

