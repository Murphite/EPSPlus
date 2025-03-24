
using EPSPlus.Domain.Entities;

namespace EPSPlus.Domain.IRepositories;

public interface IMemberRepository
{
    Task<Member> RegisterMemberAsync(Member member);
    Task<Member?> GetMemberByIdAsync(string memberId);
    Task UpdateMemberAsync(Member member);
    Task SoftDeleteMemberAsync(string memberId);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsPhoneUniqueAsync(string phone);
}
