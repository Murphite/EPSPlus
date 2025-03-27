

using EPSPlus.Application.DTOs;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Application.Interface;

public interface IContributionService
{
    Task<ServerResponse<string>> AddMonthlyContributionAsync(ContributionDto contributionDto);
    Task<ServerResponse<string>> AddVoluntaryContributionAsync(ContributionDto contributionDto);
    Task<ServerResponse<List<GetContributionDto>>> GetContributionsByMemberIdAsync(string memberId);
    Task<ServerResponse<ContributionStatementDto>> GetContributionStatementAsync(string memberId);
    Task<ServerResponse<bool>> CheckBenefitEligibilityAsync(string memberId);
    Task<List<Member>> GetAllMembersWithContributionsAsync();
    Task<ServerResponse<List<Contribution>>> ValidateMemberContributionsAsync(string memberId);
}
