using EPSPlus.Application.Interface;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EPSPlus.API.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BackgroundJobController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IContributionJobService _contributionJobService;
    private readonly ILogger<ContributionController> _logger;

    public BackgroundJobController(
        IBackgroundJobClient backgroundJobClient,
        IRecurringJobManager recurringJobManager,
        IContributionJobService contributionJobService,
        ILogger<ContributionController> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
        _contributionJobService = contributionJobService;
        _logger = logger;
    }

    /// <summary>
    /// Fire-and-forget job to validate contributions.
    /// </summary>
    [HttpPost("validate")]
    public IActionResult ValidateContributions()
    {
        _backgroundJobClient.Enqueue(() => _contributionJobService.ValidateContributionsAsync());
        return Ok("Contribution validation job queued.");
    }

    /// <summary>
    /// Manually trigger benefit eligibility updates as a fire-and-forget job.
    /// </summary>
    [HttpPost("benefit-updates")]
    public IActionResult GenerateBenefitEligibilityUpdates()
    {
        _backgroundJobClient.Enqueue(() => _contributionJobService.GenerateBenefitEligibilityUpdatesAsync());
        return Ok("Benefit eligibility update job queued.");
    }

    /// <summary>
    /// Manually trigger failed transactions and notifications handling as a fire-and-forget job.
    /// </summary>
    [HttpPost("handle-failed-transactions")]
    public IActionResult HandleFailedTransactions()
    {
        _backgroundJobClient.Enqueue(() => _contributionJobService.HandleFailedTransactionsAndNotificationsAsync());
        return Ok("Failed transactions handling job queued.");
    }

    /// <summary>
    /// Schedule recurring job for contribution validation.
    /// </summary>
    [HttpPost("schedule-validation")]
    public IActionResult ScheduleValidation()
    {
        _recurringJobManager.AddOrUpdate("validate-contributions",
            () => _contributionJobService.ValidateContributionsAsync(), Cron.Daily);

        return Ok("Recurring job for contribution validation scheduled.");
    }

    /// <summary>
    /// Schedule recurring job for benefit updates.
    /// </summary>
    [HttpPost("schedule-benefit-updates")]
    public IActionResult ScheduleBenefitUpdates()
    {
        _recurringJobManager.AddOrUpdate("benefit-updates",
            () => _contributionJobService.GenerateBenefitEligibilityUpdatesAsync(), Cron.Weekly);

        return Ok("Recurring job for benefit updates scheduled.");
    }

    /// <summary>
    /// Schedule recurring job for handling failed transactions.
    /// </summary>
    [HttpPost("schedule-failed-transactions")]
    public IActionResult ScheduleFailedTransactions()
    {
        _recurringJobManager.AddOrUpdate("failed-transactions",
            () => _contributionJobService.HandleFailedTransactionsAndNotificationsAsync(), Cron.Hourly);

        return Ok("Recurring job for failed transactions scheduled.");
    }
}