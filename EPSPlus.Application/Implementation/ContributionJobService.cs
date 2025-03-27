
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EPSPlus.Application.Implementation;

public class ContributionJobService : IContributionJobService
{
    private readonly ILogger<ContributionJobService> _logger;
    private readonly IContributionService _contributionService;
    private readonly IUnitOfWork _unitOfWork;

    public ContributionJobService(ILogger<ContributionJobService> logger, IContributionService contributionService, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _contributionService = contributionService;
        _unitOfWork = unitOfWork;
    }

    public async Task ValidateContributionsAsync()
    {
        _logger.LogInformation("Starting contribution validation...");

        var members = await _contributionService.GetAllMembersWithContributionsAsync();

        foreach (var member in members)
        {
            var contributionStatus = await _contributionService.ValidateMemberContributionsAsync(member.Id); 

            if (!contributionStatus.IsSuccessful) 
            {
                _logger.LogWarning($"Member {member.Id} has INVALID contributions: {contributionStatus.ResponseMessage}");
            }
            else
            {
                _logger.LogInformation($"Member {member.Id} has valid contributions.");
            }
        }

        _logger.LogInformation("Contribution validation completed.");
    }

    public async Task GenerateBenefitEligibilityUpdatesAsync()
    {
        _logger.LogInformation("Generating benefit eligibility updates...");

        var members = await _contributionService.GetAllMembersWithContributionsAsync();

        foreach (var member in members)
        {
            var response = await _contributionService.CheckBenefitEligibilityAsync(member.Id);

            if (!response.IsSuccessful)
            {
                _logger.LogWarning($"Member {member.Id} is NOT eligible for benefits: {response.ResponseMessage}");
            }
            else
            {
                _logger.LogInformation($"Member {member.Id} is eligible for benefits.");
            }
        }

        _logger.LogInformation("Benefit eligibility updates completed.");
    }

    public async Task HandleFailedTransactionsAndNotificationsAsync()
    {
        _logger.LogInformation("Handling failed transactions...");

        var failedContributions = await _unitOfWork.Contributions.GetFailedContributionsAsync();

        foreach (var contribution in failedContributions)
        {
            // Attempt reprocessing or notify user
            _logger.LogWarning($"Failed contribution found: {contribution.Id} for {contribution.MemberId}");
        }

        _logger.LogInformation("Failed transactions handled successfully.");
    }

}
