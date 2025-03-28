

using EPSPlus.Application.DTOs;
using EPSPlus.Application.Implementation;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Interfaces;
using Moq;

namespace EPSPlus.Tests.Services;

public class MemberServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MemberService _memberService;

    public MemberServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _memberService = new MemberService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task UpdateMemberAsync_ShouldReturnSuccess_WhenMemberExists()
    {
        // Arrange
        var memberId = "user3";
        var memberDto = new UpdateMemberDto { Id = memberId, FullName = "Murphy Unit Testing", DateOfBirth = new DateTime(1990, 5, 20), IsActive = true };
        var member = new Member
        {
            Id = memberId,
            User = new ApplicationUser
            {
                FullName = "Murphy Ogbeide",
                MemberDetails = new Member { DateOfBirth = new DateTime(1990, 5, 20) }
            }
        };

        _unitOfWorkMock.Setup(u => u.Members.GetMemberByIdAsync(memberId))
            .ReturnsAsync(member);

        _unitOfWorkMock.Setup(u => u.Members.UpdateMemberAsync(It.IsAny<Member>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _memberService.UpdateMemberAsync(memberDto);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal("Member updated successfully.", result.ResponseMessage);
        Assert.Equal(34, member.User.MemberDetails.Age); // Assuming test is in 2024
        _unitOfWorkMock.Verify(u => u.Members.UpdateMemberAsync(It.IsAny<Member>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMemberAsync_ShouldReturnFailure_WhenMemberNotFound()
    {
        // Arrange
        var memberId = "123";
        var memberDto = new UpdateMemberDto { Id = memberId };

        _unitOfWorkMock.Setup(u => u.Members.GetMemberByIdAsync(memberId))
            .ReturnsAsync((Member)null);

        // Act
        var result = await _memberService.UpdateMemberAsync(memberDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("404", result.ResponseCode);
        Assert.Equal($"No member found with ID {memberId}.", result.ResponseMessage);
    }

    [Fact]
    public async Task UpdateMemberAsync_ShouldHandleNullUserReference()
    {
        // Arrange
        var memberId = "123";
        var memberDto = new UpdateMemberDto { Id = memberId, FullName = "John Doe" };
        var member = new Member { Id = memberId, User = null };

        _unitOfWorkMock.Setup(u => u.Members.GetMemberByIdAsync(memberId))
            .ReturnsAsync(member);

        // Act
        var result = await _memberService.UpdateMemberAsync(memberDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("400", result.ResponseCode);
    }

    [Fact]
    public async Task UpdateMemberAsync_ShouldHandleNullMemberDetailsReference()
    {
        // Arrange
        var memberId = "123";
        var memberDto = new UpdateMemberDto { Id = memberId, DateOfBirth = new DateTime(1990, 1, 1) };
        var member = new Member { Id = memberId, User = new ApplicationUser { MemberDetails = null } };

        _unitOfWorkMock.Setup(u => u.Members.GetMemberByIdAsync(memberId))
            .ReturnsAsync(member);

        // Act
        var result = await _memberService.UpdateMemberAsync(memberDto);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("400", result.ResponseCode);
    }

    [Fact]
    public async Task UpdateMemberAsync_ShouldReturnZeroAge_WhenDateOfBirthIsToday()
    {
        // Arrange
        var memberId = "123";
        var today = DateTime.Today;
        var memberDto = new UpdateMemberDto { Id = memberId, DateOfBirth = today };
        var member = new Member { Id = memberId, User = new ApplicationUser { MemberDetails = new Member() } };

        _unitOfWorkMock.Setup(u => u.Members.GetMemberByIdAsync(memberId))
            .ReturnsAsync(member);

        // Act
        var result = await _memberService.UpdateMemberAsync(memberDto);

        // Assert
        Assert.Equal(0, member.User.MemberDetails.Age);
    }
}

