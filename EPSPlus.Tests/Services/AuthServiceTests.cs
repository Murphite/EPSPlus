

using EPSPlus.Application.DTOs;
using EPSPlus.Application.Implementation;
using EPSPlus.Domain.Constants;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPSPlus.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _jwtServiceMock = new Mock<IJwtService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _configurationMock = new Mock<IConfiguration>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _jwtServiceMock.Object,
            _configurationMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _httpContextAccessorMock.Object
        );
    }

    #region Login Tests

    [Fact]
    public async Task Login_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginUserDto { Email = "test@example.com", Password = "password123" };
        _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _authService.Login(loginDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Auth.Error", result.ErrorResponse.ResponseMessage);
        Assert.Equal("Your email is incorrect", result.ErrorResponse.ResponseDescription);
    }

    [Fact]
    public async Task Login_ShouldReturnError_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@example.com", IsActive = true };
        _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(false);

        var loginDto = new LoginUserDto { Email = "test@example.com", Password = "wrongpassword" };

        // Act
        var result = await _authService.Login(loginDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Auth.Error", result.ErrorResponse.ResponseMessage);
        Assert.Equal("Your password is incorrect", result.ErrorResponse.ResponseDescription);
    }

    [Fact]
    public async Task Login_ShouldReturnError_WhenUserIsInactive()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@example.com", IsActive = false };
        _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        var loginDto = new LoginUserDto { Email = "test@example.com", Password = "password123" };

        // Act
        var result = await _authService.Login(loginDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Auth.Error", result.ErrorResponse.ResponseMessage);
        Assert.Equal("Your account is inactive. Please contact support.", result.ErrorResponse.ResponseDescription);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@example.com", IsActive = true, FullName = "John Doe" };
        var roles = new List<string> { "User" };
        _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
        _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(roles);
        _jwtServiceMock.Setup(j => j.GenerateToken(user, roles)).Returns("mock_token");

        var loginDto = new LoginUserDto { Email = "test@example.com", Password = "password123" };

        // Act
        var result = await _authService.Login(loginDto);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal("Login successful", result.ResponseMessage);
        Assert.Equal("mock_token", result.Data.Token);
    }

    #endregion

    #region ResetPassword Tests

    [Fact]
    public async Task ResetPassword_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

        var resetPasswordDto = new ResetPasswordDto
        {
            NewPassword = "newpassword",
            ConfirmPassword = "newpassword"
        };

        // Act
        var result = await _authService.ResetPassword(resetPasswordDto, "test@example.com");

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Auth.UserNotFound", result.ErrorResponse.ResponseMessage);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnError_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@example.com" };
        _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        var resetPasswordDto = new ResetPasswordDto
        {
            NewPassword = "newpassword",
            ConfirmPassword = "mismatch"
        };

        // Act
        var result = await _authService.ResetPassword(resetPasswordDto, "test@example.com");

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Auth.PasswordMismatch", result.ErrorResponse.ResponseMessage);
    }

    #endregion

    #region RegisterMemberAsync Tests

    [Fact]
    public async Task RegisterMemberAsync_ShouldReturnError_WhenAgeIsInvalid()
    {
        // Arrange
        var registerMemberDto = new RegisterMemberDto
        {
            DateOfBirth = DateTime.Today.AddYears(-17), // Under 18
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            EmployerId = "1"
        };

        // Act
        var result = await _authService.RegisterMemberAsync(registerMemberDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Invalid Age", result.ErrorResponse.ResponseMessage);
    }

    [Fact]
    public async Task RegisterMemberAsync_ShouldReturnError_WhenEmployerDoesNotExist()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Employers.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Employer)null);

        var registerMemberDto = new RegisterMemberDto
        {
            DateOfBirth = DateTime.Today.AddYears(-25),
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            EmployerId = "1"
        };

        // Act
        var result = await _authService.RegisterMemberAsync(registerMemberDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Invalid Employer", result.ErrorResponse.ResponseMessage);
    }

    #endregion

    #region RegisterEmployerAsync Tests

    [Fact]
    public async Task RegisterEmployerAsync_ShouldReturnError_WhenRegistrationNumberIsEmpty()
    {
        // Arrange
        var employerDto = new RegisterEmployerDto { RegistrationNumber = "" };

        // Act
        var result = await _authService.RegisterEmployerAsync(employerDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Invalid Registration Number", result.ErrorResponse.ResponseMessage);
    }

    [Fact]
    public async Task RegisterEmployerAsync_ShouldReturnError_WhenRegistrationNumberIsDuplicate()
    {
        // Arrange
        var employerDto = new RegisterEmployerDto { RegistrationNumber = "12345" };
        _unitOfWorkMock.Setup(u => u.Employers.IsRegistrationNumberUniqueAsync(employerDto.RegistrationNumber))
            .ReturnsAsync(false);

        // Act
        var result = await _authService.RegisterEmployerAsync(employerDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Duplicate Registration Number", result.ErrorResponse.ResponseMessage);
    }

    [Fact]
    public async Task RegisterEmployerAsync_ShouldReturnError_WhenCompanyNameIsDuplicate()
    {
        // Arrange
        var employerDto = new RegisterEmployerDto
        {
            CompanyName = "Test Company",
            RegistrationNumber = "1000000",  
            Email = "testing@example.com",     
            Password = "SecurePassword123"  
        };

        _unitOfWorkMock.Setup(u => u.Employers.IsFullNameUniqueAsync(employerDto.CompanyName))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.Employers.IsRegistrationNumberUniqueAsync(employerDto.RegistrationNumber))
            .ReturnsAsync(true); 

        _unitOfWorkMock.Setup(u => u.Employers.IsEmailUniqueAsync(employerDto.Email))
            .ReturnsAsync(true); 

        // Act
        var result = await _authService.RegisterEmployerAsync(employerDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("This company name is already in use.", result.ErrorResponse.ResponseDescription);
    }

    [Fact]
    public async Task RegisterEmployerAsync_ShouldReturnError_WhenEmailIsDuplicate()
    {
        // Arrange
        var employerDto = new RegisterEmployerDto
        {
            CompanyName = "Tested Company",
            RegistrationNumber = "1000000",
            Email = "tested@example.com",
            Password = "SecurePassword123"
        };

        _unitOfWorkMock.Setup(u => u.Employers.IsFullNameUniqueAsync(employerDto.CompanyName))
            .ReturnsAsync(true);  // Ensure company name is unique so it doesn't fail there

        _unitOfWorkMock.Setup(u => u.Employers.IsRegistrationNumberUniqueAsync(employerDto.RegistrationNumber))
            .ReturnsAsync(true); // Ensure registration number is unique

        _unitOfWorkMock.Setup(u => u.Employers.IsEmailUniqueAsync(employerDto.Email))
            .ReturnsAsync(false); // Now it will fail at email validation

        // Act
        var result = await _authService.RegisterEmployerAsync(employerDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Email is already in use.", result.ErrorResponse.ResponseDescription);
    }

    #endregion

    #region RegisterAdminAsync Tests

    [Fact]
    public async Task RegisterAdminAsync_ShouldReturnError_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var registerAdminDto = new RegisterAdminDto
        {
            Email = "admin@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password456!", // Mismatched password
            PhoneNumber = "1234567890",
            FullName = "Admin User"
        };

        // Act
        var result = await _authService.RegisterAdminAsync(registerAdminDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Passwords do not match", result.ErrorResponse.ResponseMessage);
    }

    [Fact]
    public async Task RegisterAdminAsync_ShouldReturnError_WhenEmailIsNotUnique()
    {
        // Arrange
        var registerAdminDto = new RegisterAdminDto
        {
            Email = "admin@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            PhoneNumber = "1234567890",
            FullName = "Admin User"
        };

        _unitOfWorkMock.Setup(u => u.Admins.IsEmailUniqueAsync(registerAdminDto.Email))
            .ReturnsAsync(false); // Email already exists

        // Act
        var result = await _authService.RegisterAdminAsync(registerAdminDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Duplicate Email", result.ErrorResponse.ResponseMessage);
    }

    
    #endregion
}