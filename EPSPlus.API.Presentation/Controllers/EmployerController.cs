using EPSPlus.Application.DTOs;
using EPSPlus.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EPSPlus.API.Presentation.Controllers;

[ApiController]
//[Authorize]
[Route("api/[controller]")]
public class EmployerController : Controller
{
    private readonly IEmployerService _employerService;

    public EmployerController(IEmployerService employerService)
    {
        _employerService = employerService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterEmployer([FromBody] EmployerDto employerDto)
    {
        var response = await _employerService.RegisterEmployerAsync(employerDto);
        if (!response.IsSuccessful)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{employerId}")]
    public async Task<IActionResult> GetEmployerById(string employerId)
    {
        var response = await _employerService.GetEmployerByIdAsync(employerId);
        if (!response.IsSuccessful)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateEmployer([FromBody] EmployerDto employerDto)
    {
        var response = await _employerService.UpdateEmployerAsync(employerDto);
        if (!response.IsSuccessful)
            return NotFound(response);

        return Ok(response);
    }
}
