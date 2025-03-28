﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSPlus.Application.Interface;

public interface IContributionJobService
{
    Task ValidateContributionsAsync();
    Task GenerateBenefitEligibilityUpdatesAsync();
    Task HandleFailedTransactionsAndNotificationsAsync();
}
