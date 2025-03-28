
using EPSPlus.Application.DTOs;
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Domain.Responses;
using EPSPlus.Infrastructure.Repositories;
using System.ComponentModel.DataAnnotations;

namespace EPSPlus.Application.Implementation;

public class ContributionService : IContributionService
{
    private readonly IUnitOfWork _unitOfWork;

    public ContributionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse<string>> AddMonthlyContributionAsync(ContributionDto contributionDto)
    {
        if (contributionDto.Amount <= 0)
        {
            return ServerResponseExtensions.Failure<string>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Invalid Contribution",
                ResponseDescription = "Amount must be greater than zero."
            }, 400);
        }

        // 🚨 Check if the user already has a monthly contribution for this month
        var existingMonthlyContribution = await _unitOfWork.Contributions.GetMonthlyContributionAsync(contributionDto.MemberId, contributionDto.ContributionDate);

        if (existingMonthlyContribution != null)
        {
            return ServerResponseExtensions.Failure<string>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Duplicate Monthly Contribution",
                ResponseDescription = "A monthly contribution has already been made for this period."
            }, 400);
        }

        var contribution = new Contribution
        {
            MemberId = contributionDto.MemberId,
            Amount = contributionDto.Amount,
            ContributionDate = contributionDto.ContributionDate,
            ContributionType = ContributionStatus.Monthly,
            Status = "Confirmed"
        };

        await _unitOfWork.Contributions.AddMonthlyContributionAsync(contribution);
        await _unitOfWork.SaveChangesAsync(); // ✅ Commit transaction

        return new ServerResponse<string>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Monthly contribution added successfully.",
            Data = "Success"
        };
    }

    public async Task<ServerResponse<string>> AddVoluntaryContributionAsync(ContributionDto contributionDto)
    {
        if (contributionDto.Amount <= 0)
        {
            return ServerResponseExtensions.Failure<string>(new ErrorResponse
            {
                ResponseCode = "400",
                ResponseMessage = "Invalid Contribution",
                ResponseDescription = "Amount must be greater than zero."
            }, 400);
        }

        var contribution = new Contribution
        {
            MemberId = contributionDto.MemberId,
            Amount = contributionDto.Amount,
            ContributionDate = contributionDto.ContributionDate,
            ContributionType = ContributionStatus.Voluntary,
            Status = "Confirmed"
        };

        await _unitOfWork.Contributions.AddVoluntaryContributionAsync(contribution);
        await _unitOfWork.SaveChangesAsync();

        return new ServerResponse<string>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Voluntary contribution added successfully.",
            Data = "Success"
        };
    }

    public async Task<ServerResponse<List<GetContributionDto>>> GetContributionsByMemberIdAsync(string memberId)
    {
        var contributions = await _unitOfWork.Contributions.GetContributionsByMemberIdAsync(memberId);

        if (contributions == null || !contributions.Any())
        {
            return ServerResponseExtensions.Failure<List<GetContributionDto>>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "No Contributions Found",
                ResponseDescription = $"No contributions found for member ID {memberId}."
            }, 404);
        }

        var contributionsDto = contributions.Select(c => new GetContributionDto
        {
            MemberId = c.MemberId,
            Amount = c.Amount,
            ContributionDate = c.ContributionDate,
            ContributionType = c.ContributionType.ToString()
        }).ToList();

        return new ServerResponse<List<GetContributionDto>>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Contributions retrieved successfully.",
            Data = contributionsDto
        };
    }

    public async Task<ServerResponse<ContributionStatementDto>> GetContributionStatementAsync(string memberId)
    {
        var contributions = await _unitOfWork.Contributions.GetContributionsByMemberIdAsync(memberId);

        if (contributions == null || !contributions.Any())
        {
            return ServerResponseExtensions.Failure<ContributionStatementDto>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "No Contribution Statement Found",
                ResponseDescription = $"No contribution statement found for member ID {memberId}."
            }, 404);
        }

        // 🔢 Calculate total contributions
        decimal totalContributions = contributions.Sum(c => c.Amount);

        var statementDto = new ContributionStatementDto
        {
            MemberId = memberId,
            TotalContributions = totalContributions, // ✅ Sum of contributions
            Contributions = contributions.Select(c => new ContributionDto
            {
                MemberId = c.MemberId,
                Amount = c.Amount,
                ContributionDate = c.ContributionDate,
                ContributionType = c.ContributionType.ToString()
            }).ToList()
        };

        return new ServerResponse<ContributionStatementDto>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Contribution statement retrieved successfully.",
            Data = statementDto
        };
    }

    public async Task<ServerResponse<bool>> CheckBenefitEligibilityAsync(string memberId)
    {
        var contributions = await _unitOfWork.Contributions.GetContributionsByMemberIdAsync(memberId);

        if (contributions == null || !contributions.Any())
        {
            return ServerResponseExtensions.Failure<bool>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "No Contributions Found",
                ResponseDescription = "The member has not made any contributions."
            }, 404);
        }

        // ✅ Business Rule: Check if the member has contributed for at least 12 months
        var uniqueContributionMonths = contributions
            .Where(c => c.ContributionType == ContributionStatus.Monthly)
            .Select(c => new { c.ContributionDate.Year, c.ContributionDate.Month })
            .Distinct()
            .Count();

        bool isEligible = uniqueContributionMonths >= 12;

        if (!isEligible)
        {
            return ServerResponseExtensions.Failure<bool>(new ErrorResponse
            {
                ResponseCode = "403",
                ResponseMessage = "Not Eligible",
                ResponseDescription = "The member has not met the minimum contribution period required for benefit eligibility."
            }, 403);
        }

        return new ServerResponse<bool>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Member is eligible for benefits.",
            Data = true
        };
    }

    public async Task<List<Member>> GetAllMembersWithContributionsAsync()
    {
        return await _unitOfWork.Contributions.GetAllContributingMembersAsync();
    }

    public async Task<ServerResponse<List<Contribution>>> ValidateMemberContributionsAsync(string memberId)
    {
        var contributions = await _unitOfWork.Contributions.GetMemberContributionsAsync(memberId);

        if (contributions == null || !contributions.Any())
        {
            return ServerResponseExtensions.Failure<List<Contribution>>(
                new ErrorResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "No contributions found for this member."
                },
                404
            );
        }
                
        var isValid = contributions.Sum(c => c.Amount) > 0; 

        if (!isValid)
        {
            return ServerResponseExtensions.Failure<List<Contribution>>(
                new ErrorResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Insufficient contributions."
                },
                400
            );
        }

        return new ServerResponse<List<Contribution>>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Valid contributions.",
            Data = contributions
        };
    }



}
