
using EPSPlus.Application.DTOs;
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Entities;
using EPSPlus.Domain.Enum;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Domain.Responses;

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

        var contribution = new Contribution
        {
            MemberId = contributionDto.MemberId,
            Amount = contributionDto.Amount,
            ContributionDate = contributionDto.ContributionDate,
            ContributionType = ContributionStatus.Monthly
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
            ContributionType = ContributionStatus.Voluntary
        };

        await _unitOfWork.Contributions.AddVoluntaryContributionAsync(contribution);
        await _unitOfWork.SaveChangesAsync(); // ✅ Commit transaction

        return new ServerResponse<string>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Voluntary contribution added successfully.",
            Data = "Success"
        };
    }

    public async Task<ServerResponse<List<ContributionDto>>> GetContributionsByMemberIdAsync(string memberId)
    {
        var contributions = await _unitOfWork.Contributions.GetContributionsByMemberIdAsync(memberId);

        if (contributions == null || !contributions.Any())
        {
            return ServerResponseExtensions.Failure<List<ContributionDto>>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "No Contributions Found",
                ResponseDescription = $"No contributions found for member ID {memberId}."
            }, 404);
        }

        var contributionsDto = contributions.Select(c => new ContributionDto
        {
            MemberId = c.MemberId,
            Amount = c.Amount,
            ContributionDate = c.ContributionDate
        }).ToList();

        return new ServerResponse<List<ContributionDto>>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Contributions retrieved successfully.",
            Data = contributionsDto
        };
    }

    public async Task<ServerResponse<ContributionStatementDto>> GetContributionStatementAsync(string memberId)
    {
        var statement = await _unitOfWork.Contributions.GenerateContributionStatementAsync(memberId);

        if (statement == null)
        {
            return ServerResponseExtensions.Failure<ContributionStatementDto>(new ErrorResponse
            {
                ResponseCode = "404",
                ResponseMessage = "No Contribution Statement Found",
                ResponseDescription = $"No contribution statement found for member ID {memberId}."
            }, 404);
        }

        var statementDto = new ContributionStatementDto
        {
            MemberId = statement.MemberId,
            TotalContributions = statement.TotalContributions,
            Contributions = statement.Contributions.Select(c => new ContributionDto
            {
                MemberId = c.MemberId,
                Amount = c.Amount,
                ContributionDate = c.ContributionDate
            }).ToList() // ✅ Convert to List<ContributionDto>
        };

        return new ServerResponse<ContributionStatementDto>
        {
            IsSuccessful = true,
            ResponseCode = "200",
            ResponseMessage = "Contribution statement retrieved successfully.",
            Data = statementDto
        };
    }

}
