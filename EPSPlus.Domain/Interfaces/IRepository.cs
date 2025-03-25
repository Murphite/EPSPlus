

using Microsoft.EntityFrameworkCore.Storage;

namespace EPSPlus.Domain.Interfaces;

public interface IRepository
{
    Task<IDbContextTransaction> BeginTransactionAsync();
}
