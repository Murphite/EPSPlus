

namespace EPSPlus.Domain.IRepositories;

public interface ITransactionRepository
{
    Task ValidateContributionsAsync();
    Task UpdateBenefitEligibilityAsync();
    Task ProcessFailedTransactionsAsync();
    Task SendNotificationsAsync();
}