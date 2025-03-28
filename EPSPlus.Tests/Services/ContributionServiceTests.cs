using Moq;
using EPSPlus.Application.DTOs;
using EPSPlus.Application.Implementation;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using EPSPlus.Domain.Interfaces;
using FluentAssertions;

namespace EPSPlus.Tests.Services;

public class ContributionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ContributionService _contributionService;

    public ContributionServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _contributionService = new ContributionService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddMonthlyContributionAsync_ShouldReturnError_WhenAmountIsZeroOrNegative()
    {
        // Arrange
        var contributionDto = new ContributionDto { Amount = 0, MemberId = "123", ContributionDate = DateTime.UtcNow };

        // Act
        var result = await _contributionService.AddMonthlyContributionAsync(contributionDto);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ResponseCode.Should().Be("400");
        result.ResponseMessage.Should().Be("Amount must be greater than zero.");
    }

    [Fact]
    public async Task AddMonthlyContributionAsync_ShouldReturnError_WhenDuplicateContributionExists()
    {
        // Arrange
        var contributionDto = new ContributionDto { Amount = 100, MemberId = "123", ContributionDate = DateTime.UtcNow };
        _unitOfWorkMock.Setup(u => u.Contributions.GetMonthlyContributionAsync(contributionDto.MemberId, contributionDto.ContributionDate))
                       .ReturnsAsync(new Contribution());

        // Act
        var result = await _contributionService.AddMonthlyContributionAsync(contributionDto);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ResponseCode.Should().Be("400");
        result.ResponseMessage.Should().Be("A monthly contribution has already been made for this period.");
    }

    [Fact]
    public async Task AddMonthlyContributionAsync_ShouldAddContribution_WhenValid()
    {
        // Arrange
        var contributionDto = new ContributionDto { Amount = 100, MemberId = "123", ContributionDate = DateTime.UtcNow };
        _unitOfWorkMock.Setup(u => u.Contributions.GetMonthlyContributionAsync(contributionDto.MemberId, contributionDto.ContributionDate))
                       .ReturnsAsync((Contribution)null);

        // Act
        var result = await _contributionService.AddMonthlyContributionAsync(contributionDto);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.ResponseCode.Should().Be("200");
        result.ResponseMessage.Should().Be("Monthly contribution added successfully.");
        _unitOfWorkMock.Verify(u => u.Contributions.AddMonthlyContributionAsync(It.IsAny<Contribution>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddVoluntaryContributionAsync_ShouldReturnError_WhenAmountIsZeroOrNegative()
    {
        // Arrange
        var contributionDto = new ContributionDto { Amount = 0, MemberId = "123", ContributionDate = DateTime.UtcNow };

        // Act
        var result = await _contributionService.AddVoluntaryContributionAsync(contributionDto);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ResponseCode.Should().Be("400");
        result.ResponseMessage.Should().Be("Amount must be greater than zero.");
    }

    [Fact]
    public async Task AddVoluntaryContributionAsync_ShouldAddContribution_WhenValid()
    {
        // Arrange
        var contributionDto = new ContributionDto { Amount = 150, MemberId = "1", ContributionDate = DateTime.UtcNow, ContributionType = "Voluntary", };
        _unitOfWorkMock.Setup(u => u.Contributions.GetVoluntaryContributionAsync(contributionDto.MemberId, contributionDto.ContributionDate))
                       .ReturnsAsync((Contribution)null);

        // Act
        var result = await _contributionService.AddVoluntaryContributionAsync(contributionDto);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.ResponseCode.Should().Be("200");
        result.ResponseMessage.Should().Be("Voluntary contribution added successfully.");       
    }

    [Fact]
    public async Task GetContributionsByMemberIdAsync_ShouldReturnError_WhenNoContributionsFound()
    {
        // Arrange
        string memberId = "123";
        _unitOfWorkMock.Setup(u => u.Contributions.GetContributionsByMemberIdAsync(memberId))
                       .ReturnsAsync(new List<Contribution>());

        // Act
        var result = await _contributionService.GetContributionsByMemberIdAsync(memberId);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ResponseCode.Should().Be("404");
        result.ResponseMessage.Should().Be($"No contributions found for member ID {memberId}.");
    }

    [Fact]
    public async Task GetContributionsByMemberIdAsync_ShouldReturnContributions_WhenFound()
    {
        // Arrange
        string memberId = "123";
        var contributions = new List<Contribution>
        {
            new Contribution { MemberId = memberId, Amount = 100, ContributionDate = DateTime.UtcNow, ContributionType = ContributionStatus.Monthly },
            new Contribution { MemberId = memberId, Amount = 150, ContributionDate = DateTime.UtcNow, ContributionType = ContributionStatus.Voluntary }
        };
        _unitOfWorkMock.Setup(u => u.Contributions.GetContributionsByMemberIdAsync(memberId))
                       .ReturnsAsync(contributions);

        // Act
        var result = await _contributionService.GetContributionsByMemberIdAsync(memberId);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.ResponseCode.Should().Be("200");
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetContributionStatementAsync_ShouldReturnError_WhenNoContributionsFound()
    {
        // Arrange
        string memberId = "123";
        _unitOfWorkMock.Setup(u => u.Contributions.GetContributionsByMemberIdAsync(memberId))
                       .ReturnsAsync(new List<Contribution>());

        // Act
        var result = await _contributionService.GetContributionStatementAsync(memberId);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ResponseCode.Should().Be("404");
        result.ResponseMessage.Should().Be($"No contribution statement found for member ID {memberId}.");
    }

    [Fact]
    public async Task GetContributionStatementAsync_ShouldReturnStatement_WhenContributionsExist()
    {
        // Arrange
        string memberId = "123";
        var contributions = new List<Contribution>
        {
            new Contribution { MemberId = memberId, Amount = 200, ContributionDate = DateTime.UtcNow, ContributionType = ContributionStatus.Monthly },
            new Contribution { MemberId = memberId, Amount = 300, ContributionDate = DateTime.UtcNow, ContributionType = ContributionStatus.Voluntary }
        };
        _unitOfWorkMock.Setup(u => u.Contributions.GetContributionsByMemberIdAsync(memberId))
                       .ReturnsAsync(contributions);

        // Act
        var result = await _contributionService.GetContributionStatementAsync(memberId);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.ResponseCode.Should().Be("200");
        result.Data.Should().NotBeNull();
        result.Data.TotalContributions.Should().Be(500);
        result.Data.Contributions.Should().HaveCount(2);
    }
}