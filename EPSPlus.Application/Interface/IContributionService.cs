

using EPSPlus.Application.DTOs;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Application.Interface;

public interface IContributionService
{
    Task<ServerResponse<string>> AddMonthlyContributionAsync(ContributionDto contributionDto);
    Task<ServerResponse<string>> AddVoluntaryContributionAsync(ContributionDto contributionDto);
    Task<ServerResponse<List<ContributionDto>>> GetContributionsByMemberIdAsync(string memberId);
    Task<ServerResponse<ContributionStatementDto>> GetContributionStatementAsync(string memberId);

}
