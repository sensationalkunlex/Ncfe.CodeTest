using System.Collections.Generic;

namespace Ncfe.CodeTest
{
    public interface IFailoverReviewService
    {
        bool DetermineFailover(List<FailoverEntry> failoverEntries);
    }
}
