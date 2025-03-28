
using EPSPlus.Application.DTOs;
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Constants;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Domain.Responses;
using Microsoft.AspNetCore.Identity;

namespace EPSPlus.Application.Implementation;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;

    public MemberService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

   
    public async Task<ServerResponse<MemberDto>> GetMemberByIdAsync(string memberId)
    {
        var member = await _unitOfWork.Members.GetMemberByIdAsync(memberId);

        if (member == null)
        {
            return ServerResponseExtensions.Failure<MemberDto>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "Member Not Found",
                ResponseDescription = $"No member found with ID {memberId}."
            }, 404);
        }

        var memberDto = new MemberDto
        {
            Id = member.Id,
            ActiveStatus = member.User.IsActive,
            FullName = member.User.FullName,
            Email = member.User.Email,
            PhoneNumber = member.User.PhoneNumber,
            DateOfBirth = member.DateOfBirth,
            Age = DateTime.Now.Year - member.DateOfBirth.Year,
            CompanyName = member.Employer?.CompanyName
        };

        return new ServerResponse<MemberDto>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Member retrieved successfully.",
            Data = memberDto
        };
    }

    public async Task<ServerResponse<IEnumerable<MemberDto>>> GetAllMembersAsync()
    {
        var members = await _unitOfWork.Members.GetAllMembersAsync();

        if (!members.Any())
        {
            return ServerResponseExtensions.Failure<IEnumerable<MemberDto>>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "No Members Found",
                ResponseDescription = "There are no registered members."
            }, 404);
        }

        var memberDtos = members.Select(member => new MemberDto
        {
            Id = member.Id,
            FullName = member.User.FullName,
            Email = member.User.Email,
            DateOfBirth = member.DateOfBirth,
            PhoneNumber = member.User.PhoneNumber,
            Age = DateTime.Now.Year - member.DateOfBirth.Year,
            CompanyName = member.Employer?.CompanyName,
            ActiveStatus = member.User.IsActive
        }).ToList();

        return new ServerResponse<IEnumerable<MemberDto>>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Members retrieved successfully.",
            Data = memberDtos
        };
    }


    public async Task<ServerResponse<UpdateMemberDto>> UpdateMemberAsync(UpdateMemberDto memberDto)
    {
        var member = await _unitOfWork.Members.GetMemberByIdAsync(memberDto.Id);

        if (member == null)
        {
            return ServerResponseExtensions.Failure<UpdateMemberDto>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "Member Not Found",
                ResponseDescription = $"No member found with ID {memberDto.Id}."
            }, 404);
        }

        if (member.User != null)
        {
            member.User.PhoneNumber = memberDto.PhoneNumber;
            member.User.FullName = memberDto.FullName;
            member.User.Email = memberDto.Email;
            member.User.IsActive = memberDto.IsActive;

            if (member.User.MemberDetails != null)
            {
                member.User.MemberDetails.DateOfBirth = memberDto.DateOfBirth;
                member.User.MemberDetails.EmployerId = memberDto.EmployerId;
                member.User.MemberDetails.Age = CalculateAge(memberDto.DateOfBirth);
            }
            else
            {
                return ServerResponseExtensions.Failure<UpdateMemberDto>(new ErrorResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "MemberDetails Not Found",
                    ResponseDescription = $"No member details found for member ID {memberDto.Id}."
                }, 400);
            }
        }
        else
        {
            return ServerResponseExtensions.Failure<UpdateMemberDto>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "User Not Found",
                ResponseDescription = $"The member with ID {memberDto.Id} does not have a linked user."
            }, 400);
        }

        await _unitOfWork.Members.UpdateMemberAsync(member);

        return new ServerResponse<UpdateMemberDto>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Member updated successfully.",
            Data = memberDto
        };
    }

    // Method to calculate age
    private int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }


    public async Task<ServerResponse<string>> SoftDeleteMemberAsync(string memberId)
    {
        var member = await _unitOfWork.Members.GetMemberByIdAsync(memberId);

        if (member == null)
        {
            return ServerResponseExtensions.Failure<string>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "Member Not Found",
                ResponseDescription = $"No member found with ID {memberId}."
            }, 404);
        }

        // Ensure User is not null before accessing IsActive
        if (member.User != null)
        {
            member.User.IsActive = false;
            await _unitOfWork.Members.UpdateMemberAsync(member);
        }
        else
        {
            return ServerResponseExtensions.Failure<string>(new ErrorResponse
            {
                ResponseCode = "500",
                ResponseMessage = "User Not Found",
                ResponseDescription = $"No associated user found for member ID {memberId}."
            }, 500);
        }

        return new ServerResponse<string>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Member deactivated successfully.",
            Data = $"Member {memberId} has been set to Inactive."
        };
    }



}
