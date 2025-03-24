using EPSPlus.Domain.Entities;
using EPSPlus.Domain.IRepositories;
using EPSPlus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace EPSPlus.Infrastructure.Repositories;

public class EmployerRepository : IEmployerRepository
{
    private readonly AppDbContext _context;

    public EmployerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Employer> RegisterEmployerAsync(Employer employer)
    {
        if (string.IsNullOrWhiteSpace(employer.RegistrationNumber))
            throw new ValidationException("Employer must have a valid registration number.");

        if (!employer.ActiveStatus)
            throw new ValidationException("Employer must be active.");

        _context.Employers.Add(employer);
        await _context.SaveChangesAsync();
        return employer;
    }

    public async Task<Employer?> GetEmployerByIdAsync(string employerId)
    {
        return await _context.Employers
            .Include(e => e.Members)
            .FirstOrDefaultAsync(e => e.Id == employerId);
    }

    public async Task UpdateEmployerAsync(Employer employer)
    {
        _context.Employers.Update(employer);
        await _context.SaveChangesAsync();
    }
}
