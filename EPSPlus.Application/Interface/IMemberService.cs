

using EPSPlus.Application.DTOs;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Application.Interface;

public interface IMemberService
{
    //Task<ServerResponse<MemberDto>> RegisterMemberAsync(MemberDto memberDto);
    Task<ServerResponse<MemberDto>> GetMemberByIdAsync(string memberId);
    Task<ServerResponse<MemberDto>> UpdateMemberAsync(MemberDto memberDto);
    Task<ServerResponse<string>> SoftDeleteMemberAsync(string memberId);
}
