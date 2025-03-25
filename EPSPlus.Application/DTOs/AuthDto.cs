

using System.ComponentModel.DataAnnotations;

namespace EPSPlus.Application.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; set; }

    public LoginResponseDto(string token, string fullName, IList<string> roles, string email)
    {
        Token = token;
        FullName = fullName;
        Roles = roles;
        Email = email;
    }
}

public record LoginUserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}


public class ResetPasswordDto
{
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}

public class PasswordResetResponseDto
{
    public string? Email { get; set; }
    public bool IsPasswordReset { get; set; } = true;
    public string? Message { get; set; }
    public string? NewPassword { get; set; }
}

public class RegisterMemberDto
{
    public string? FullName { get; set; }
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    [Range(18, 70, ErrorMessage = "Member must be between 18 and 70 years old.")]
    public int Age { get; set; }
    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
    public string ConfirmPassword { get; set; } = default!;
}

public class RegisterMemberResponseDto
{
    public string? UserId { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string? Role { get; set; }
    public string? Token { get; set; }
}

public class RegisterEmployerDto
{
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string RegistrationNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
    public string ConfirmPassword { get; set; } = default!;
}


public class RegisterEmployerResponseDto
{
    public string? Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool ActiveStatus { get; set; }
    public string? Role { get; set; }
    public string? Token { get; set; }
}