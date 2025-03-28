using EPSPlus.Application.DTOs;
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Application.Implementation;

public class EmployerService : IEmployerService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse<List<EmployerDto>>> GetAllEmployersAsync()
    {
        var employers = await _unitOfWork.Employers.GetAllAsync();

        if (employers == null || !employers.Any())
        {
            return ServerResponseExtensions.Failure<List<EmployerDto>>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "No Employers Found",
                ResponseDescription = "There are no employers available in the system."
            }, 404);
        }

        var employerDtos = employers.Select(employer => new EmployerDto
        {
            Id = employer.Id,
            CompanyName = employer.CompanyName!,
            RegistrationNumber = employer.RegistrationNumber!,
            ActiveStatus = employer.User!.IsActive,
            Members = employer.Members?.Select(m => new EmployerMembersDto
            {
                Id = m.User?.MemberDetails?.Id,
                FullName = m.User != null ? m.User.FullName : "N/A",
                Email = m.User != null ? m.User.Email : "N/A",
                PhoneNumber = m.User != null ? m.User.PhoneNumber! : "N/A",
                DateOfBirth = m.User!.MemberDetails!.DateOfBirth,
                Age = m.User?.MemberDetails?.Age ?? 0,
                ActiveStatus = m.User!.IsActive,
            }).ToList() ?? new List<EmployerMembersDto>()
        }).ToList();

        return new ServerResponse<List<EmployerDto>>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Employers retrieved successfully.",
            Data = employerDtos
        };
    }


    public async Task<ServerResponse<EmployerDto>> GetEmployerByIdAsync(string employerId)
    {
        var employer = await _unitOfWork.Employers.GetEmployerByIdAsync(employerId);

        if (employer == null)
        {
            return ServerResponseExtensions.Failure<EmployerDto>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "Employer Not Found",
                ResponseDescription = $"No employer found with ID {employerId}."
            }, 404);
        }

        var employerDto = new EmployerDto
        {
            Id = employer.Id,
            CompanyName = employer.CompanyName,
            ActiveStatus = employer.User!.IsActive,
            RegistrationNumber = employer.RegistrationNumber,
            Members = employer.Members?.Select(m => new EmployerMembersDto
            {
                Id = m.User.MemberDetails.Id,
                FullName = m.User != null ? m.User.FullName : "N/A",
                Email = m.User != null ? m.User.Email : "N/A",
                PhoneNumber = m.User != null ? m.User.PhoneNumber : "N/A",
                DateOfBirth = m.User.MemberDetails.DateOfBirth,
                ActiveStatus = m.User!.IsActive,
                Age = m.User.MemberDetails.Age
            }).ToList() ?? new List<EmployerMembersDto>()
        };

        return new ServerResponse<EmployerDto>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Employer retrieved successfully.",
            Data = employerDto
        };
    }


    public async Task<ServerResponse<string>> UpdateEmployerAsync(UpdateEmployerDto employerDto)
    {
        var employer = await _unitOfWork.Employers.GetEmployerByIdAsync(employerDto.Id);

        if (employer == null)
        {
            return ServerResponseExtensions.Failure<string>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "Employer Not Found",
                ResponseDescription = $"No employer found with ID {employerDto.Id}."
            }, 404);
        }

        if (employer.User == null)
        {
            return ServerResponseExtensions.Failure<string>(new ErrorResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Employer user reference is null.",
                ResponseDescription = $"The employer with ID {employerDto.Id} has no associated user."
            }, 500);
        }


        employer.CompanyName = employerDto.CompanyName;
        employer.RegistrationNumber = employerDto.RegistrationNumber;
        employer.User.IsActive = employerDto.ActiveStatus;

        await _unitOfWork.Employers.UpdateEmployerAsync(employer);

        return new ServerResponse<string>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Employer updated successfully.",
            Data = "Success"
        };
    }


}

