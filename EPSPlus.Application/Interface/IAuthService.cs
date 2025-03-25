

using EPSPlus.Application.DTOs;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Application.Interface;

public interface IAuthService
{
    Task<ServerResponse<LoginResponseDto>> Login(LoginUserDto loginUserDto);
    Task<ServerResponse<PasswordResetResponseDto>> ResetPassword(ResetPasswordDto resetPasswordDto, string email);
    Task<ServerResponse<RegisterMemberResponseDto>> RegisterMemberAsync(RegisterMemberDto registerMemberDto);
    Task<ServerResponse<RegisterEmployerResponseDto>> RegisterEmployerAsync(RegisterEmployerDto employerDto);
}
