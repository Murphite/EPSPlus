

using EPSPlus.Application.DTOs;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Application.Interface;

public interface IMemberService
{
    Task<ServerResponse<IEnumerable<MemberDto>>> GetAllMembersAsync();
    Task<ServerResponse<MemberDto>> GetMemberByIdAsync(string memberId);
    Task<ServerResponse<UpdateMemberDto>> UpdateMemberAsync(UpdateMemberDto memberDto);
    Task<ServerResponse<string>> SoftDeleteMemberAsync(string memberId);
}
