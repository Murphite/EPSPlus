using EPSPlus.Application.Implementation;
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Domain.Responses;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPSPlus.Tests.Services;

public class ContributionJobServiceTests
{
    private readonly Mock<ILogger<ContributionJobService>> _loggerMock;
    private readonly Mock<IContributionService> _contributionServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ContributionJobService _contributionJobService;

    public ContributionJobServiceTests()
    {
        _loggerMock = new Mock<ILogger<ContributionJobService>>();
        _contributionServiceMock = new Mock<IContributionService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _contributionJobService = new ContributionJobService(
            _loggerMock.Object,
            _contributionServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task ValidateContributionsAsync_ShouldValidateEachMemberContribution()
    {
        // Arrange
        var members = new List<Member>
        {
            new Member { Id = "1" },
            new Member { Id = "2" }
        };

        var contributions = new List<Contribution>
        {
            new Contribution { Amount = 100 }
        };

        _contributionServiceMock.Setup(s => s.GetAllMembersWithContributionsAsync())
            .ReturnsAsync(members);

        _contributionServiceMock.Setup(s => s.ValidateMemberContributionsAsync(It.IsAny<string>()))
            .ReturnsAsync(new ServerResponse<List<Contribution>>
            {
                IsSuccessful = true,
                ResponseCode = "200",
                ResponseMessage = "Valid contributions.",
                Data = contributions
            });

        // Act
        await _contributionJobService.ValidateContributionsAsync();

        // Assert
        _contributionServiceMock.Verify(s => s.ValidateMemberContributionsAsync("1"), Times.Once);
        _contributionServiceMock.Verify(s => s.ValidateMemberContributionsAsync("2"), Times.Once);
    }

    [Fact]
    public async Task GenerateBenefitEligibilityUpdatesAsync_ShouldCheckEligibilityForEachMember()
    {
        // Arrange
        var members = new List<Member>
        {
            new Member { Id = "1" },
            new Member { Id = "2" }
        };

        _contributionServiceMock.Setup(s => s.GetAllMembersWithContributionsAsync())
            .ReturnsAsync(members);

        _contributionServiceMock.Setup(s => s.CheckBenefitEligibilityAsync(It.IsAny<string>()))
            .ReturnsAsync(new ServerResponse<bool> { IsSuccessful = true, ResponseMessage = "Success" });

        // Act
        await _contributionJobService.GenerateBenefitEligibilityUpdatesAsync();

        // Assert
        _contributionServiceMock.Verify(s => s.CheckBenefitEligibilityAsync("1"), Times.Once);
        _contributionServiceMock.Verify(s => s.CheckBenefitEligibilityAsync("2"), Times.Once);
    }

    [Fact]
    public async Task HandleFailedTransactionsAndNotificationsAsync_ShouldProcessFailedTransactions()
    {
        // Arrange
        var failedContributions = new List<Contribution>
        {
            new Contribution { Id = "1001", MemberId = "1" },
            new Contribution { Id = "1002", MemberId = "2" }
        };

        _unitOfWorkMock.Setup(u => u.Contributions.GetFailedContributionsAsync())
            .ReturnsAsync(failedContributions);

        // Act
        await _contributionJobService.HandleFailedTransactionsAndNotificationsAsync();

        // Assert
        _unitOfWorkMock.Verify(u => u.Contributions.GetFailedContributionsAsync(), Times.Once);
    }
}
