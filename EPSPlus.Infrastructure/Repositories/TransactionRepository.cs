using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace EPSPlus.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task ValidateContributionsAsync()
    {
        var invalidContributions = await _context.Contributions
            .Where(c => c.Amount <= 0)
            .ToListAsync();

        foreach (var contribution in invalidContributions)
        {
            contribution.Status = "Invalid";
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateBenefitEligibilityAsync()
    {
        var eligibleMembers = await _context.Members
            .Where(m => m.User.IsActive == true) // Check User's IsActive status
            .ToListAsync();

        foreach (var member in eligibleMembers)
        {
            // Enforce minimum contribution period
            var contributions = await _context.Contributions
                .Where(c => c.MemberId == member.Id)
                .CountAsync();

            if (contributions >= 12) // 1-year minimum contributions
            {
                var eligibility = new BenefitEligibility
                {
                    MemberId = member.Id,
                    EligibleDate = DateTime.UtcNow,
                    Status = true,
                };

                _context.BenefitEligibilities.Add(eligibility);
            }
        }

        await _context.SaveChangesAsync();
    }


    public async Task ProcessFailedTransactionsAsync()
    {
        var failedTransactions = await _context.Transactions
            .Where(t => t.Status == TransactionStatus.Failed)
            .ToListAsync();

        foreach (var transaction in failedTransactions)
        {
            // Retry logic
            transaction.Status = TransactionStatus.Pending;

            await _context.SaveChangesAsync();
        }
    }

    public async Task SendNotificationsAsync()
    {
        // Implement notification logic here (e.g., email, SMS)
    }
}

