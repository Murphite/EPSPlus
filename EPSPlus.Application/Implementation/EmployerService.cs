

using EPSPlus.Application.DTOs;
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Domain.Responses;
using EPSPlus.Infrastructure.Persistence;

namespace EPSPlus.Application.Implementation;

public class EmployerService : IEmployerService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse<EmployerDto>> RegisterEmployerAsync(EmployerDto employerDto)
    {
        if (string.IsNullOrWhiteSpace(employerDto.RegistrationNumber))
        {
            return ServerResponseExtensions.Failure<EmployerDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Invalid Registration Number",
                ResponseDescription = "Employer must have a valid registration number."
            }, 400);
        }

        if (!employerDto.ActiveStatus)
        {
            return ServerResponseExtensions.Failure<EmployerDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Inactive Employer",
                ResponseDescription = "Employer must be active."
            }, 400);
        }

        var employer = new Employer
        {
            Id = Guid.NewGuid().ToString(),
            CompanyName = employerDto.CompanyName,
            RegistrationNumber = employerDto.RegistrationNumber
        };

        var registeredEmployer = await _unitOfWork.Employers.AddEmployerAsync(employer);

        return new ServerResponse<EmployerDto>
        {
            IsSuccessful = true,
            ResponseCode = "201",
            ResponseMessage = "Employer registered successfully.",
            Data = new EmployerDto
            {
                Id = registeredEmployer.Id,
                CompanyName = registeredEmployer.CompanyName,
                RegistrationNumber = registeredEmployer.RegistrationNumber
            }
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
            RegistrationNumber = employer.RegistrationNumber,
            Members = employer.Members?.Select(m => new MemberDto
            {
                FullName = m.User.FullName,
                Email = m.User.Email,
                PhoneNumber = m.User.PhoneNumber
            }).ToList()
        };

        return new ServerResponse<EmployerDto>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Employer retrieved successfully.",
            Data = employerDto
        };
    }

    public async Task<ServerResponse<string>> UpdateEmployerAsync(EmployerDto employerDto)
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

