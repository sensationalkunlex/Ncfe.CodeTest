using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncfe.CodeTest
{
    public class LearnerService: ILearnerService
    {
        private readonly IArchivedDataService _archivedDataService;
        private readonly ILearnerDataAccess _learnerDataAccess;
        private readonly IFailoverLearnerDataAccess _failoverLearnerDataAccess;
        private readonly IFailoverRepository _failoverRepository;
        private readonly IFailoverReview _failoverReview;
        public LearnerService(
            IArchivedDataService archivedDataService, 
            ILearnerDataAccess learnerDataAccess, 
            IFailoverLearnerDataAccess failoverLearnerDataAccess, 
            IFailoverRepository failoverRepository, 
            IFailoverReview failoverReview)
        {
            _archivedDataService = archivedDataService;
            _learnerDataAccess = learnerDataAccess;
            _failoverLearnerDataAccess = failoverLearnerDataAccess;
            _failoverRepository = failoverRepository;
            _failoverReview = failoverReview;
        }

        public Learner GetLearner(int learnerId, bool isLearnerArchived)
        {
           
            if (isLearnerArchived)
            {
                return _archivedDataService.GetArchivedLearner(learnerId);
            }
            else
            {   
                var failoverEntries = _failoverRepository.GetFailOverEntries();
                LearnerResponse learnerResponse = null;
               
                if (_failoverReview.DetermineFailover(failoverEntries))
                {
                    learnerResponse = _failoverLearnerDataAccess.GetLearnerById(learnerId);
                }
                else
                {
                    learnerResponse = _learnerDataAccess.LoadLearner(learnerId);
                }

                var result = learnerResponse.IsArchived? 
                    _archivedDataService.GetArchivedLearner(learnerId)
                    :learnerResponse.Learner;

                return result;
            }
        }
  
    }

}



