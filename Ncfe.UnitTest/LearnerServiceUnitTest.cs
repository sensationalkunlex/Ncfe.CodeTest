using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ncfe.CodeTest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ncfe.UnitTest
{
    /// <summary>
    /// Summary description for LearnerServiceUnitTest
    /// </summary>
    [TestClass]
    public class LearnerServiceUnitTest
    {
        public LearnerServiceUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private Mock<IArchivedDataService> _archivedDataServiceMockService;
        private Mock<ILearnerDataAccess> _learnerDataAccessMockService;
        private Mock<IFailoverLearnerDataAccess> _failoverLearnerDataAccessMockService;
        private Mock<IFailoverRepository> _failoverRepositoryMockService;
        private Mock<IFailoverReview> _failoverReviewMockService;
        private LearnerService _learnerService;

        [TestInitialize]
        public void SetUp()
        {
            _archivedDataServiceMockService = new Mock<IArchivedDataService>();
            _learnerDataAccessMockService = new Mock<ILearnerDataAccess>();
            _failoverLearnerDataAccessMockService = new Mock<IFailoverLearnerDataAccess>();
            _failoverRepositoryMockService = new Mock<IFailoverRepository>();
            _failoverReviewMockService = new Mock<IFailoverReview>();

            _learnerService = new LearnerService(
                _archivedDataServiceMockService.Object,
                _learnerDataAccessMockService.Object,
                _failoverLearnerDataAccessMockService.Object,
                _failoverRepositoryMockService.Object,
                _failoverReviewMockService.Object
            );
        }


        /// <summary>
        /// This test ensures that when failover is enabled, 
        /// the GetLearner method returns the learner from the failover data access.
        /// </summary>

        [TestMethod]
        public void GetLearnerReturnsFailoverIfWhenEnabled()
        {
           
            int learnerId = 2;
            var failoverEntries = new List<FailoverEntry>();
            var expectedLearner = new Learner();
            var learnerResponse = new LearnerResponse { Learner = expectedLearner };

           
            _failoverRepositoryMockService.Setup(r => r.GetFailOverEntries()).Returns(failoverEntries);
            _failoverReviewMockService.Setup(r => r.DetermineFailover(failoverEntries)).Returns(true);
            _failoverLearnerDataAccessMockService.Setup(d => d.GetLearnerById(learnerId)).Returns(learnerResponse);

           
            var result = _learnerService.GetLearner(learnerId, false);
            Assert.AreEqual(expectedLearner, result);
        }




        /// <summary>
        /// To test  when the isLearnerArchived parameter is set to true,
        /// the learner from the archived data service.
        /// </summary>
        [TestMethod]
        public void GetArchivedLearnerIfIsLearnerArchivedEqualTrue()
        {

            int learnerId = 2;
            var expectedLearner = new Learner();
            _archivedDataServiceMockService.Setup(y => y.GetArchivedLearner(learnerId)).Returns(expectedLearner);
            var result = _learnerService.GetLearner(learnerId, true);
            Assert.AreEqual(expectedLearner, result);
        }

        /// <summary>
        /// This test demonstrates that the GetLearner function returns the learner
        /// from failover data access when failover is enabled.
        /// </summary>

        [TestMethod]
        public void GetLearnerReturnsFailoverWhenTrue()
        {
            
            int learnerId = 5;
            var failoverLists = new List<FailoverEntry>();
            var expectedResult = new Learner();
            var learnerResponse = new LearnerResponse { Learner = expectedResult };



            _failoverRepositoryMockService.Setup(y => y.GetFailOverEntries()).Returns(failoverLists);
            _failoverReviewMockService.Setup(y => y.DetermineFailover(failoverLists)).Returns(true);
            _failoverLearnerDataAccessMockService.Setup(y => y.GetLearnerById(learnerId)).Returns(learnerResponse);

            var result = _learnerService.GetLearner(learnerId, false);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
