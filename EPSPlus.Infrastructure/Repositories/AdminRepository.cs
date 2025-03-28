using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EPSPlus.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;

    public AdminRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Admin> AddAdminAsync(Admin admin)
    {
        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();
        return admin;
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email);
    }
}

