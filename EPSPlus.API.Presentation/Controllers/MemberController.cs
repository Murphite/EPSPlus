using EPSPlus.Application.DTOs;
using EPSPlus.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPSPlus.API.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly ILogger<MemberController> _logger;

    public MemberController(IMemberService memberService,
        ILogger<MemberController> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }    

   
    [HttpGet("{memberId}")]
    public async Task<IActionResult> GetMemberById(string memberId)
    {
        var response = await _memberService.GetMemberByIdAsync(memberId);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }

    
    [HttpPut("update")]
    public async Task<IActionResult> UpdateMember([FromBody] MemberDto memberDto)
    {
        var response = await _memberService.UpdateMemberAsync(memberDto);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }

   
    [HttpDelete("soft-delete/{memberId}")]
    public async Task<IActionResult> SoftDeleteMember(string memberId)
    {
        var response = await _memberService.SoftDeleteMemberAsync(memberId);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }

}