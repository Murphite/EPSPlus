

using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Domain.IRepositories;

public interface IContributionRepository
{
    Task AddMonthlyContributionAsync(Contribution contribution);
    Task AddVoluntaryContributionAsync(Contribution contribution);
    Task<IEnumerable<Contribution>> GetContributionsByMemberIdAsync(string memberId);
    Task<ContributionStatement> GenerateContributionStatementAsync(string memberId);
}
