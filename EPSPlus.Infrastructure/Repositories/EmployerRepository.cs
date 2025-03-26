using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Interfaces;
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

    public async Task<Employer> AddEmployerAsync(Employer employer)
    {        
        _context.Employers.Add(employer);
        await _context.SaveChangesAsync();

        return employer;
    }

    public async Task<Employer> GetEmployerByIdAsync(string employerId)
    {
        var employers =  await _context.Employers
            .Include(e => e.Members) 
            .ThenInclude(m => m.User) 
            .FirstOrDefaultAsync(e => e.Id == employerId);

        return employers!;
    }


    public async Task UpdateEmployerAsync(Employer employer)
    {
        _context.Employers.Update(employer);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsFullNameUniqueAsync(string name)
    {
        return !await _context.Users.AnyAsync(u => u.FullName == name);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone)
    {
        return !await _context.Users.AnyAsync(u => u.PhoneNumber == phone);
    }

    public async Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber)
    {
        return !await _context.Employers.AnyAsync(u => u.RegistrationNumber == registrationNumber);
    }
}
