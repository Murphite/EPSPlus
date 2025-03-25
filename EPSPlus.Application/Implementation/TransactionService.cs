

using EPSPlus.Application.Interface;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Domain.Responses;

namespace EPSPlus.Application.Implementation;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse<string>> ValidateContributionsAsync()
    {
        try
        {
            await _unitOfWork.Transactions.ValidateContributionsAsync();
            return new ServerResponse<string>
            {
                IsSuccessful = true,
                ResponseCode = "200",
                ResponseMessage = "Contribution validation completed.",
                Data = "Success"
            };
        }
        catch (Exception ex)
        {
            return ServerResponseExtensions.Failure<string>(
                new ErrorResponse
                {
                    ResponseCode = "500",
                    ResponseMessage = "Contribution validation failed",
                    ResponseDescription = ex.Message
                }, 500);
        }
    }

    public async Task<ServerResponse<string>> UpdateBenefitEligibilityAsync()
    {
        try
        {
            await _unitOfWork.Transactions.UpdateBenefitEligibilityAsync();
            return new ServerResponse<string>
            {
                IsSuccessful = true,
                ResponseCode = "200",
                ResponseMessage = "Benefit eligibility updated.",
                Data = "Success"
            };
        }
        catch (Exception ex)
        {
            return ServerResponseExtensions.Failure<string>(
                new ErrorResponse
                {
                    ResponseCode = "500",
                    ResponseMessage = "Benefit eligibility update failed",
                    ResponseDescription = ex.Message
                }, 500);
        }
    }

    public async Task<ServerResponse<string>> ProcessFailedTransactionsAsync()
    {
        try
        {
            await _unitOfWork.Transactions.ProcessFailedTransactionsAsync();
            return new ServerResponse<string>
            {
                IsSuccessful = true,
                ResponseCode = "200",
                ResponseMessage = "Failed transactions processed.",
                Data = "Success"
            };
        }
        catch (Exception ex)
        {
            return ServerResponseExtensions.Failure<string>(
                new ErrorResponse
                {
                    ResponseCode = "500",
                    ResponseMessage = "Failed transactions processing failed",
                    ResponseDescription = ex.Message
                }, 500);
        }
    }
}