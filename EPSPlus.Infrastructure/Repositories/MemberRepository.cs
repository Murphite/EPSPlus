using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Infrastructure.Persistence;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace EPSPlus.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Member> AddMemberAsync(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<Member?> GetMemberByIdAsync(string memberId)
    {
        return await _context.Members
            .Include(m => m.User)
            .Include(m => m.Employer)
            .FirstOrDefaultAsync(m => m.Id == memberId);
    }

    public async Task UpdateMemberAsync(Member member)
    {
        _context.Members.Update(member);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone)
    {
        return !await _context.Users.AnyAsync(u => u.PhoneNumber == phone);
    }

    public async Task<bool> IsFullNameUniqueAsync(string name)
    {
        return !await _context.Users.AnyAsync(u => u.FullName == name);
    }
}

