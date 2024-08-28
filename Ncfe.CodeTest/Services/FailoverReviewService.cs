using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncfe.CodeTest
{
    public class FailoverReviewService: IFailoverReviewService
    {
        private readonly TimeSpan _failoverPeriod;
        private readonly int _failoverLimit;

        public FailoverReviewService(TimeSpan failoverPeriod, int failoverLimit)
        {
            _failoverPeriod = failoverPeriod;
            _failoverLimit = failoverLimit;
        }

        public bool DetermineFailover(List<FailoverEntry> failoverEntries)
        {
            DateTime durationRange = DateTime.UtcNow.Subtract(_failoverPeriod);
            int failureCount = failoverEntries.Count(entry => entry.DateTime > durationRange);
            bool IsFailoverModeEnabled = ConfigurationManager.AppSettings["IsFailoverModeEnabled"]?.ToLower() == "true";
            return failureCount > _failoverLimit && IsFailoverModeEnabled;
        }
    }
   

}
