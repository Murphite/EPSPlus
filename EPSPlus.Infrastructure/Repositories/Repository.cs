

using EPSPlus.Domain.Interfaces;
using EPSPlus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace EPSPlus.Infrastructure.Repositories;

public class Repository : IRepository
{
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}
