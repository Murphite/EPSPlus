
using Moq;
using EPSPlus.Application.Implementation;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Application.DTOs;

namespace EPSPlus.Tests.Services;

public class EmployerServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly EmployerService _employerService;

    public EmployerServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _employerService = new EmployerService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task UpdateEmployerAsync_ShouldReturnSuccess_WhenEmployerExists()
    {
        // Arrange
        var employerId = "123";
        var employerDto = new UpdateEmployerDto { Id = employerId, CompanyName = "New Name", RegistrationNumber = "98765", ActiveStatus = true };
        var employer = new Employer { Id = employerId, CompanyName = "Old Name", RegistrationNumber = "12345", User = new ApplicationUser { IsActive = false } };

        _unitOfWorkMock.Setup(u => u.Employers.GetEmployerByIdAsync(employerId))
            .ReturnsAsync(employer);

        _unitOfWorkMock.Setup(u => u.Employers.UpdateEmployerAsync(It.IsAny<Employer>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _employerService.UpdateEmployerAsync(employerDto);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal("Employer updated successfully.", result.ResponseMessage);
        _unitOfWorkMock.Verify(u => u.Employers.UpdateEmployerAsync(It.IsAny<Employer>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployerAsync_ShouldReturnFailure_WhenEmployerNotFound()
    {
        // Arrange
        var employerId = "123";
        var employerDto = new UpdateEmployerDto { Id = employerId, CompanyName = "New Name" };

        _unitOfWorkMock.Setup(u => u.Employers.GetEmployerByIdAsync(employerId))
            .ReturnsAsync((Employer)null);

        // Act
        var result = await _employerService.UpdateEmployerAsync(employerDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("404", result.ResponseCode);
        Assert.Equal($"No employer found with ID {employerId}.", result.ResponseMessage);
    }


    [Fact]
    public async Task UpdateEmployerAsync_ShouldHandleNullUserReference()
    {
        // Arrange
        var employerId = "123";
        var employerDto = new UpdateEmployerDto { Id = employerId, CompanyName = "New Name", ActiveStatus = true };
        var employer = new Employer { Id = employerId, User = null };

        _unitOfWorkMock.Setup(u => u.Employers.GetEmployerByIdAsync(employerId))
            .ReturnsAsync(employer);

        // Act
        var result = await _employerService.UpdateEmployerAsync(employerDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("500", result.ResponseCode);
    }
}
