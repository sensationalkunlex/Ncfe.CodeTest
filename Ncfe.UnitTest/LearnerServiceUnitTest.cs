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

        private Mock<IArchivedDataService> _archivedDataServiceMockService;
        private Mock<ILearnerDataAccess> _learnerDataAccessMockService;
        private Mock<IFailoverRepository> _failoverRepositoryMockService;
        private Mock<IFailoverReviewService> _failoverReviewMockService;
        private LearnerService _learnerService;

        [TestInitialize]
        public void SetUp()
        {
            _archivedDataServiceMockService = new Mock<IArchivedDataService>();
            _learnerDataAccessMockService = new Mock<ILearnerDataAccess>();
            _failoverRepositoryMockService = new Mock<IFailoverRepository>();
            _failoverReviewMockService = new Mock<IFailoverReviewService>();

            _learnerService = new LearnerService(
                _archivedDataServiceMockService.Object,
                _learnerDataAccessMockService.Object,
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
public class LearnerServiceTests
{
    private readonly Mock<IArchivedDataService> _mockArchivedDataService;
    private readonly Mock<ILearnerDataAccess> _mockLearnerDataAccess;
    private readonly Mock<IFailoverRepository> _mockFailoverRepository;
    private readonly Mock<IFailoverReviewService> _mockFailoverReviewService;
    private readonly LearnerService _learnerService;

    public LearnerServiceTests()
    {
        // Arrange: Create mock objects for dependencies
        _mockArchivedDataService = new Mock<IArchivedDataService>();
        _mockLearnerDataAccess = new Mock<ILearnerDataAccess>();
        _mockFailoverRepository = new Mock<IFailoverRepository>();
        _mockFailoverReviewService = new Mock<IFailoverReviewService>();

        // Arrange: Initialize the service with the mocked dependencies
        _learnerService = new LearnerService(
            _mockArchivedDataService.Object,
            _mockLearnerDataAccess.Object,
            _mockFailoverRepository.Object,
            _mockFailoverReviewService.Object
        );
    }

    [Fact]
    public void GetLearner_ShouldReturnArchivedLearner_WhenIsLearnerArchivedIsTrue()
    {
        // Arrange
        int learnerId = 1;
        var archivedLearner = new Learner();
        _mockArchivedDataService
            .Setup(s => s.GetArchivedLearner(learnerId))
            .Returns(archivedLearner);

        // Act
        var result = _learnerService.GetLearner(learnerId, isLearnerArchived: true);

        // Assert
        Assert.Equal(archivedLearner, result);
        _mockArchivedDataService.Verify(s => s.GetArchivedLearner(learnerId), Times.Once);
    }

    [Fact]
    public void GetLearner_ShouldReturnActiveLearner_WhenIsLearnerArchivedIsFalseAndFailoverIsNotTriggered()
    {
        // Arrange
        int learnerId = 1;
        var learnerResponse = new LearnerResponse { IsArchived = false, Learner = new Learner() };
        _mockFailoverRepository.Setup(r => r.GetFailOverEntries()).Returns(new List<FailoverEntry>());
        _mockFailoverReviewService.Setup(s => s.DetermineFailover(It.IsAny<List<FailoverEntry>>())).Returns(false);
        _mockLearnerDataAccess.Setup(d => d.LoadLearner(learnerId)).Returns(learnerResponse);

        // Act
        var result = _learnerService.GetLearner(learnerId, isLearnerArchived: false);

        // Assert
        Assert.Equal(learnerResponse.Learner, result);
        _mockLearnerDataAccess.Verify(d => d.LoadLearner(learnerId), Times.Once);
    }

    [Fact]
    public void GetLearner_ShouldReturnFailoverLearner_WhenFailoverIsTriggered()
    {
        // Arrange
        int learnerId = 1;
        var failoverEntries = new List<FailoverEntry> { new FailoverEntry() };
        var failoverLearnerResponse = new LearnerResponse { IsArchived = false, Learner = new Learner() };

        _mockFailoverRepository.Setup(r => r.GetFailOverEntries()).Returns(failoverEntries);
        _mockFailoverReviewService.Setup(s => s.DetermineFailover(failoverEntries)).Returns(true);
        _mockLearnerDataAccess.Setup(d => d.LoadLearner(learnerId)).Returns(failoverLearnerResponse);

        // Act
        var result = _learnerService.GetLearner(learnerId, isLearnerArchived: false);

        // Assert
        Assert.Equal(failoverLearnerResponse.Learner, result);
    }

    [Fact]
    public void GetLearner_ShouldReturnArchivedLearner_WhenLearnerIsArchivedAfterRetrieval()
    {
        // Arrange
        int learnerId = 1;
        var learnerResponse = new LearnerResponse { IsArchived = true };
        var archivedLearner = new Learner();
        _mockFailoverRepository.Setup(r => r.GetFailOverEntries()).Returns(new List<FailoverEntry>());
        _mockFailoverReviewService.Setup(s => s.DetermineFailover(It.IsAny<List<FailoverEntry>>())).Returns(false);
        _mockLearnerDataAccess.Setup(d => d.LoadLearner(learnerId)).Returns(learnerResponse);
        _mockArchivedDataService.Setup(s => s.GetArchivedLearner(learnerId)).Returns(archivedLearner);

        // Act
        var result = _learnerService.GetLearner(learnerId, isLearnerArchived: false);

        // Assert
        Assert.Equal(archivedLearner, result);
        _mockArchivedDataService.Verify(s => s.GetArchivedLearner(learnerId), Times.Once);
    }

    [Fact]
    public void GetLearner_ShouldThrowException_WhenExceptionIsRaised()
    {
        // Arrange
        int learnerId = 1;
        _mockArchivedDataService.Setup(s => s.GetArchivedLearner(learnerId)).Throws(new Exception("Test Exception"));

        // Act & Assert
        Assert.Throws<Exception>(() => _learnerService.GetLearner(learnerId, isLearnerArchived: true));
    }
}
}
