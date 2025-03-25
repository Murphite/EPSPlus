

namespace EPSPlus.Domain.Interfaces;

public interface ITransactionRepository
{
    Task ValidateContributionsAsync();
    Task UpdateBenefitEligibilityAsync();
    Task ProcessFailedTransactionsAsync();
    Task SendNotificationsAsync();
}