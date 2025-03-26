using EPSPlus.Application.DTOs;
using EPSPlus.Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EPSPlus.API.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ContributionController : Controller
{
    private readonly IContributionService _contributionService;

    public ContributionController(IContributionService contributionService)
    {
        _contributionService = contributionService;
    }

    [HttpPost("monthly")]
    public async Task<IActionResult> AddMonthlyContribution([FromBody] ContributionDto contributionDto)
    {
        var response = await _contributionService.AddMonthlyContributionAsync(contributionDto);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("voluntary")]
    public async Task<IActionResult> AddVoluntaryContribution([FromBody] ContributionDto contributionDto)
    {
        var response = await _contributionService.AddVoluntaryContributionAsync(contributionDto);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("member/{memberId}")]
    public async Task<IActionResult> GetContributionsByMemberId(string memberId)
    {
        var response = await _contributionService.GetContributionsByMemberIdAsync(memberId);
        if (!response.IsSuccessful)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("statement/{memberId}")]
    public async Task<IActionResult> GetContributionStatement(string memberId)
    {
        var response = await _contributionService.GetContributionStatementAsync(memberId);
        if (!response.IsSuccessful)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("check-benefit-eligibility/{memberId}")]
    public async Task<IActionResult> CheckBenefitEligibility(string memberId)
    {
        var response = await _contributionService.CheckBenefitEligibilityAsync(memberId);
        if (!response.IsSuccessful)
            return NotFound(response);

        return Ok(response);
    }
}
