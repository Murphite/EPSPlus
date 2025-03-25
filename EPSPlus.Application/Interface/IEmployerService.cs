
using EPSPlus.Application.DTOs;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Application.Interface;

public interface IEmployerService
{
    Task<ServerResponse<EmployerDto>> RegisterEmployerAsync(EmployerDto employerDto);
    Task<ServerResponse<EmployerDto>> GetEmployerByIdAsync(string employerId);
    Task<ServerResponse<string>> UpdateEmployerAsync(EmployerDto employerDto);
}
