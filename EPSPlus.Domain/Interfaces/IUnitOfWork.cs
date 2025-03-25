

namespace EPSPlus.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IContributionRepository Contributions { get; }
    IEmployerRepository Employers { get; }
    IMemberRepository Members { get; }
    ITransactionRepository Transactions { get; }
    IRepository Repository { get; }
    Task<int> SaveChangesAsync();
}