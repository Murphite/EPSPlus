

using EPSPlus.Domain.Responses;

namespace EPSPlus.Application.Interface;

public interface ITransactionService
{
    Task<ServerResponse<string>> ValidateContributionsAsync();
    Task<ServerResponse<string>> UpdateBenefitEligibilityAsync();
    Task<ServerResponse<string>> ProcessFailedTransactionsAsync();
}
