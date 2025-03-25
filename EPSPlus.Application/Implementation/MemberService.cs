
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
            FullName = member.User.FullName,
            Email = member.User.Email,
            PhoneNumber = member.User.PhoneNumber,
            Age = DateTime.UtcNow.Year - member.DateOfBirth.Year,
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


    public async Task<ServerResponse<MemberDto>> UpdateMemberAsync(MemberDto memberDto)
    {
        var member = await _unitOfWork.Members.GetMemberByIdAsync(memberDto.Id);

        if (member == null)
        {
            return ServerResponseExtensions.Failure<MemberDto>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "Member Not Found",
                ResponseDescription = $"No member found with ID {memberDto.Id}."
            }, 404);
        }

        member.User.PhoneNumber = memberDto.PhoneNumber;
        member.User.Email = memberDto.Email;
        member.Employer.CompanyName = memberDto.CompanyName;
        member.User.IsActive = memberDto.ActiveStatus;


        await _unitOfWork.Members.UpdateMemberAsync(member);

        return new ServerResponse<MemberDto>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Member updated successfully.",
            Data = memberDto
        };
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
