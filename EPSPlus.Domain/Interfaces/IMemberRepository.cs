
using EPSPlus.Domain.Entities;

namespace EPSPlus.Domain.Interfaces;

public interface IMemberRepository
{
    Task<Member> AddMemberAsync(Member member);
    Task<Member?> GetMemberByIdAsync(string memberId);
    Task UpdateMemberAsync(Member member);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsPhoneUniqueAsync(string phone);
    Task<bool> IsFullNameUniqueAsync(string name);
}
