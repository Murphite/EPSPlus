using EPSPlus.Application.DTOs;
using EPSPlus.Application.Implementation;
using EPSPlus.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EPSPlus.API.Presentation.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-member")]
    public async Task<IActionResult> RegisterMember([FromBody] RegisterMemberDto memberDto)
    {
        var response = await _authService.RegisterMemberAsync(memberDto);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("register-employer")]
    public async Task<IActionResult> RegisterEmployer([FromBody] RegisterEmployerDto employerDto)
    {
        var response = await _authService.RegisterEmployerAsync(employerDto);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        var response = await _authService.Login(loginDto);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("reset-password/{email}")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto, string email)
    {
        var response = await _authService.ResetPassword(resetPasswordDto, email);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }


}
