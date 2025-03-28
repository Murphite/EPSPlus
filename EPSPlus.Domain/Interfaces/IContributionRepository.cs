

using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Domain.Interfaces;

public interface IContributionRepository
{
    Task AddMonthlyContributionAsync(Contribution contribution);
    Task AddVoluntaryContributionAsync(Contribution contribution);
    Task<IEnumerable<Contribution>> GetContributionsByMemberIdAsync(string memberId);
    Task<ContributionStatement> GenerateContributionStatementAsync(string memberId);
    Task<List<Contribution>> GetMonthlyContributionAsync(string memberId);
    Task<Contribution?> GetVoluntaryContributionAsync(string memberId, DateTime contributionDate);
    Task<Contribution?> GetMonthlyContributionAsync(string memberId, DateTime contributionDate);
    Task<List<Member>> GetAllContributingMembersAsync();
    Task<List<Contribution>> GetFailedContributionsAsync();
    Task<List<Contribution>> GetMemberContributionsAsync(string memberId);
}
