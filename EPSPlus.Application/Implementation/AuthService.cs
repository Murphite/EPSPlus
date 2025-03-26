
using EPSPlus.Application.DTOs;
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Constants;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Domain.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EPSPlus.Application.Implementation;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        ILogger<AuthService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServerResponse<LoginResponseDto>> Login(LoginUserDto loginUserDto)
    {
        _logger.LogInformation("******* Inside the Login Method ********");

        var user = await _userManager.FindByEmailAsync(loginUserDto.Email);

        if (user is null)
        {
            _logger.LogWarning("User not found for email: {email}", loginUserDto.Email);
            return new ServerResponse<LoginResponseDto>
            {
                IsSuccessful = false,
                ErrorResponse = new ErrorResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Auth.Error",
                    ResponseDescription = "Your email is incorrect"
                }
            };
        }

        if (!user.IsActive) // Check if the user is inactive
        {
            _logger.LogWarning("User {email} is inactive", user.Email);
            return new ServerResponse<LoginResponseDto>
            {
                IsSuccessful = false,
                ErrorResponse = new ErrorResponse
                {
                    ResponseCode = "403",
                    ResponseMessage = "Auth.Error",
                    ResponseDescription = "Your account is inactive. Please contact support."
                }
            };
        }

        _logger.LogInformation("User found: {email}", user.Email);

        var isValidUser = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);

        if (!isValidUser)
        {
            _logger.LogWarning("Invalid password for email: {email}", user.Email);
            return new ServerResponse<LoginResponseDto>
            {
                IsSuccessful = false,
                ErrorResponse = new ErrorResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Auth.Error",
                    ResponseDescription = "Your password is incorrect"
                }
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        // Log if user has no roles
        if (roles == null || !roles.Any())
        {
            _logger.LogWarning("User {email} has no roles assigned", user.Email);
        }

        // Generate JWT token and check if the token service works properly
        try
        {
            _logger.LogInformation("Generating token for user {email}", user.Email);
            var token = _jwtService.GenerateToken(user, roles!);

            var email = user.Email ?? string.Empty;
            var fullName = user.FullName ?? string.Empty;

            return new ServerResponse<LoginResponseDto>
            {
                IsSuccessful = true,
                ResponseCode = "00",
                ResponseMessage = "Login successful",
                Data = new LoginResponseDto(token, fullName, roles!, email)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating token for {email}", user.Email);
            return new ServerResponse<LoginResponseDto>
            {
                IsSuccessful = false,
                ErrorResponse = new ErrorResponse
                {
                    ResponseCode = "500",
                    ResponseMessage = "Auth.Error",
                    ResponseDescription = "An error occurred during login"
                }
            };
        }
    }

    public async Task<ServerResponse<PasswordResetResponseDto>> ResetPassword(ResetPasswordDto resetPasswordDto, string email)
    {
        _logger.LogInformation("******* Inside the ResetAdminPassword Method ********");

        var user = await _userManager.FindByEmailAsync(email);

        // Step 2: Ensure password and confirm password match
        if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
        {
            return new ServerResponse<PasswordResetResponseDto>
            {
                IsSuccessful = false,
                ErrorResponse = new ErrorResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Auth.Error",
                    ResponseDescription = "Passwords do not match"
                }
            };
        }

        // Step 3: Generate a password reset token and reset the password
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordDto.NewPassword!);
        if (!resetPasswordResult.Succeeded)
        {
            return new ServerResponse<PasswordResetResponseDto>
            {
                IsSuccessful = false,
                ErrorResponse = new ErrorResponse
                {
                    ResponseCode = "500",
                    ResponseMessage = "Auth.Error",
                    ResponseDescription = "Error occurred while resetting the password"
                }
            };
        }

        // Step 5: Return success response
        var passwordResetResponse = new PasswordResetResponseDto
        {
            Email = email, // Retain the email from verification
            IsPasswordReset = true,
            NewPassword = resetPasswordDto.NewPassword,
            Message = "Password reset successful"
        };

        return new ServerResponse<PasswordResetResponseDto>
        {
            IsSuccessful = true,
            ResponseCode = "00",
            ResponseMessage = "Admin password reset successful",
            Data = passwordResetResponse
        };
    }

    public async Task<ServerResponse<RegisterMemberResponseDto>> RegisterMemberAsync(RegisterMemberDto registerMemberDto)
    {
        if (registerMemberDto.Age < 18 || registerMemberDto.Age > 70)
        {
            return ServerResponseExtensions.Failure<RegisterMemberResponseDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Invalid Age",
                ResponseDescription = "Member must be between 18 and 70 years old."
            }, 400);
        }

        if (registerMemberDto.Password != registerMemberDto.ConfirmPassword)
        {
            return new ServerResponse<RegisterMemberResponseDto>
            {
                IsSuccessful = false,
                ErrorResponse = new ErrorResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Passwords do not match",
                    ResponseDescription = "Password and confirmation password do not match."
                }
            };
        }

        if (!await _unitOfWork.Members.IsFullNameUniqueAsync(registerMemberDto.FullName!))
        {
            return ServerResponseExtensions.Failure<RegisterMemberResponseDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Duplicate Full Name",
                ResponseDescription = "Full name is already in use."
            }, 400);
        }

        if (!await _unitOfWork.Members.IsEmailUniqueAsync(registerMemberDto.Email))
        {
            return ServerResponseExtensions.Failure<RegisterMemberResponseDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Duplicate Email",
                ResponseDescription = "Email is already in use."
            }, 400);
        }

        if (!await _unitOfWork.Members.IsPhoneUniqueAsync(registerMemberDto.PhoneNumber))
        {
            return ServerResponseExtensions.Failure<RegisterMemberResponseDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Duplicate Phone Number",
                ResponseDescription = "Phone number is already in use."
            }, 400);
        }

        using (var transaction = await _unitOfWork.Repository.BeginTransactionAsync())
        {
            try
            {
                var applicationUser = new ApplicationUser
                {
                    FullName = registerMemberDto.FullName,
                    UserName = registerMemberDto.Email,
                    UserType = "Member",
                    Email = registerMemberDto.Email,
                    PhoneNumber = registerMemberDto.PhoneNumber,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                };

                var result = await _userManager.CreateAsync(applicationUser, registerMemberDto.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new ServerResponse<RegisterMemberResponseDto>
                    {
                        IsSuccessful = false,
                        ErrorResponse = new ErrorResponse
                        {
                            ResponseCode = "400",
                            ResponseMessage = "User creation failed",
                            ResponseDescription = string.Join("; ", result.Errors.Select(e => e.Description))
                        }
                    };
                }

                result = await _userManager.AddToRoleAsync(applicationUser, RolesConstant.Member);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new ServerResponse<RegisterMemberResponseDto>
                    {
                        IsSuccessful = false,
                        ErrorResponse = new ErrorResponse
                        {
                            ResponseCode = "400",
                            ResponseMessage = "Failed to assign role",
                            ResponseDescription = string.Join("; ", result.Errors.Select(e => e.Description))
                        }
                    };
                }

                var member = new Member
                {
                    UserId = applicationUser.Id,
                    CreatedAt = DateTime.Now,
                    User = applicationUser,
                    DateOfBirth = registerMemberDto.DateOfBirth,
                    Age = registerMemberDto.Age
                };

                await _unitOfWork.Members.AddMemberAsync(member);

                await transaction.CommitAsync();

                // Automatically log in the user
                var token = await GenerateJwtToken(applicationUser);

                return new ServerResponse<RegisterMemberResponseDto>
                {
                    IsSuccessful = true,
                    ResponseCode = "00",
                    ResponseMessage = "Registration successful",
                    Data = new RegisterMemberResponseDto
                    {
                        UserId = applicationUser.Id,
                        FullName = applicationUser.FullName,
                        Email = applicationUser.Email,
                        PhoneNumber = applicationUser.PhoneNumber,
                        IsActive = applicationUser.IsActive,
                        Age = member.Age,
                        DateOfBirth = member.DateOfBirth,
                        Role = RolesConstant.Member,
                        Token = token
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServerResponse<RegisterMemberResponseDto>
                {
                    IsSuccessful = false,
                    ErrorResponse = new ErrorResponse
                    {
                        ResponseCode = "500",
                        ResponseMessage = "Internal Server Error",
                        ResponseDescription = ex.Message
                    }
                };
            }
        }
    }

    public async Task<ServerResponse<RegisterEmployerResponseDto>> RegisterEmployerAsync(RegisterEmployerDto employerDto)
    {
        if (string.IsNullOrWhiteSpace(employerDto.RegistrationNumber))
        {
            return ServerResponseExtensions.Failure<RegisterEmployerResponseDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Invalid Registration Number",
                ResponseDescription = "Employer must have a valid registration number."
            }, 400);
        }

        if (!await _unitOfWork.Employers.IsRegistrationNumberUniqueAsync(employerDto.RegistrationNumber))
        {
            return ServerResponseExtensions.Failure<RegisterEmployerResponseDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Duplicate Registration Number",
                ResponseDescription = "This registration number is already in use."
            }, 400);
        }

        if (!await _unitOfWork.Employers.IsFullNameUniqueAsync(employerDto.CompanyName))
        {
            return ServerResponseExtensions.Failure<RegisterEmployerResponseDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Duplicate Company Name",
                ResponseDescription = "This company name is already in use."
            }, 400);
        }

        if (!await _unitOfWork.Employers.IsEmailUniqueAsync(employerDto.Email))
        {
            return ServerResponseExtensions.Failure<RegisterEmployerResponseDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Duplicate Email",
                ResponseDescription = "Email is already in use."
            }, 400);
        }

        if (!await _unitOfWork.Employers.IsPhoneUniqueAsync(employerDto.PhoneNumber))
        {
            return ServerResponseExtensions.Failure<RegisterEmployerResponseDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Duplicate Phone Number",
                ResponseDescription = "Phone number is already in use."
            }, 400);
        }

        using (var transaction = await _unitOfWork.Repository.BeginTransactionAsync())
        {
            try
            {
                var applicationUser = new ApplicationUser
                {
                    FullName = employerDto.CompanyName,
                    UserName = employerDto.Email,
                    UserType = "Employer",
                    Email = employerDto.Email,
                    PhoneNumber = employerDto.PhoneNumber,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                };

                var result = await _userManager.CreateAsync(applicationUser, employerDto.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new ServerResponse<RegisterEmployerResponseDto>
                    {
                        IsSuccessful = false,
                        ErrorResponse = new ErrorResponse
                        {
                            ResponseCode = "400",
                            ResponseMessage = "User creation failed",
                            ResponseDescription = string.Join("; ", result.Errors.Select(e => e.Description))
                        }
                    };
                }

                result = await _userManager.AddToRoleAsync(applicationUser, RolesConstant.Employer);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new ServerResponse<RegisterEmployerResponseDto>
                    {
                        IsSuccessful = false,
                        ErrorResponse = new ErrorResponse
                        {
                            ResponseCode = "400",
                            ResponseMessage = "Failed to assign role",
                            ResponseDescription = string.Join("; ", result.Errors.Select(e => e.Description))
                        }
                    };
                }

                var employer = new Employer
                {
                    Id = Guid.NewGuid().ToString(),
                    CompanyName = employerDto.CompanyName,
                    RegistrationNumber = employerDto.RegistrationNumber,
                    UserId = applicationUser.Id,
                    User = applicationUser
                };

                var registeredEmployer = await _unitOfWork.Employers.AddEmployerAsync(employer);
                await transaction.CommitAsync();

                // Automatically log in the user
                var token = await GenerateJwtToken(applicationUser);

                return new ServerResponse<RegisterEmployerResponseDto>
                {
                    IsSuccessful = true,
                    ResponseCode = "201",
                    ResponseMessage = "Employer registered successfully.",
                    Data = new RegisterEmployerResponseDto
                    {
                        Id = registeredEmployer.Id,
                        CompanyName = registeredEmployer.CompanyName,
                        RegistrationNumber = registeredEmployer.RegistrationNumber,
                        Email = applicationUser.Email,
                        PhoneNumber = applicationUser.PhoneNumber,
                        ActiveStatus = applicationUser.IsActive,
                        Role = RolesConstant.Employer,
                        Token = token
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServerResponse<RegisterEmployerResponseDto>
                {
                    IsSuccessful = false,
                    ErrorResponse = new ErrorResponse
                    {
                        ResponseCode = "500",
                        ResponseMessage = "Internal Server Error",
                        ResponseDescription = ex.Message
                    }
                };
            }
        }
    }



    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName!),
        new Claim(ClaimTypes.Email, user.Email!),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
