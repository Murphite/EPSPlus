
using EPSPlus.Domain.Interfaces;

namespace EPSPlus.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IContributionRepository Contributions { get; }
    public IEmployerRepository Employers { get; }
    public IMemberRepository Members { get; }
    public ITransactionRepository Transactions { get; }
    public IAdminRepository Admins { get; }
    public IRepository Repository { get; }

    public UnitOfWork(AppDbContext context,
                      IContributionRepository contributions,
                      IEmployerRepository employers,
                      IMemberRepository members,
                      IAdminRepository admins,
                      ITransactionRepository transactions,
                      IRepository repository)
    {
        _context = context;
        Contributions = contributions;
        Employers = employers;
        Members = members;
        Admins = admins;
        Transactions = transactions;
        Repository = repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
