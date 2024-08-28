using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncfe.CodeTest
{
    public class LearnerService : ILearnerService
    {
        private readonly IArchivedDataService _archivedDataService;
        private readonly ILearnerDataAccess _learnerDataAccess;
        private readonly IFailoverRepository _failoverRepository;
        private readonly IFailoverReviewService _failoverReview;
        public LearnerService(
            IArchivedDataService archivedDataService,
            ILearnerDataAccess learnerDataAccess,
            IFailoverRepository failoverRepository,
            IFailoverReviewService failoverReview)
        {
            _archivedDataService = archivedDataService;
            _learnerDataAccess = learnerDataAccess;
            _failoverRepository = failoverRepository;
            _failoverReview = failoverReview;
        }

        public Learner GetLearner(int learnerId, bool isLearnerArchived)
        {
            try
            {
                if (isLearnerArchived)
                {
                    return _archivedDataService.GetArchivedLearner(learnerId);
                }
                else
                {
                    var failoverEntries = _failoverRepository.GetFailOverEntries();
                    LearnerResponse learnerResponse = null;
                    learnerResponse = GetLearnerDetails(learnerId, failoverEntries);
                    var result = learnerResponse.IsArchived ?
                                _archivedDataService.GetArchivedLearner(learnerId)
                                :learnerResponse.Learner;

                    return result;
                }
            }
            catch (Exception ex)
            {
                //log in error
                throw ex;
            }
        }

        private LearnerResponse GetLearnerDetails(int learnerId, List<FailoverEntry> failoverEntries)
        {
            LearnerResponse learnerResponse;
            if (_failoverReview.DetermineFailover(failoverEntries))
            {
                learnerResponse = FailoverLearnerDataAccess.GetLearnerById(learnerId);
            }
            else
            {
                learnerResponse = _learnerDataAccess.LoadLearner(learnerId);
            }

            return learnerResponse;
        }
    }

}


