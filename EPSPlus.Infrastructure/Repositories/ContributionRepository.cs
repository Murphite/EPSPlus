using Microsoft.EntityFrameworkCore;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using EPSPlus.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using EPSPlus.Domain.Responses;
using EPSPlus.Infrastructure.Persistence;

namespace EPSPlus.Infrastructure.Repositories
{
    public class ContributionRepository : IContributionRepository
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ContributionRepository(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task AddMonthlyContributionAsync(Contribution contribution)
        {
            contribution.ContributionType = ContributionStatus.Monthly;
            await ValidateContribution(contribution);

            _context.Contributions.Add(contribution);
            await _unitOfWork.SaveChangesAsync(); // ✅ Use UnitOfWork
        }

        public async Task AddVoluntaryContributionAsync(Contribution contribution)
        {
            contribution.ContributionType = ContributionStatus.Voluntary;
            await ValidateContribution(contribution);

            _context.Contributions.Add(contribution);
            await _unitOfWork.SaveChangesAsync(); // ✅ Use UnitOfWork
        }

        public async Task<IEnumerable<Contribution>> GetContributionsByMemberIdAsync(string memberId)
        {
            return await _context.Contributions
                .Where(c => c.MemberId == memberId)
                .ToListAsync();
        }

        public async Task<ContributionStatement> GenerateContributionStatementAsync(string memberId)
        {
            var contributions = await GetContributionsByMemberIdAsync(memberId);
            var totalAmount = contributions.Sum(c => c.Amount);

            return new ContributionStatement
            {
                MemberId = memberId,
                TotalContributions = totalAmount,
                Contributions = contributions.ToList()
            };
        }

        private async Task ValidateContribution(Contribution contribution)
        {
            if (contribution.Amount <= 0)
                throw new ValidationException("Contribution amount must be greater than zero.");

            if (contribution.ContributionDate > DateTime.UtcNow)
                throw new ValidationException("Contribution date cannot be in the future.");

            var member = await _context.Members.FindAsync(contribution.MemberId);
            if (member == null)
                throw new ValidationException("Invalid Member ID.");
        }
    }
}
