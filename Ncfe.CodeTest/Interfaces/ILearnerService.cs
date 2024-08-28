
namespace Ncfe.CodeTest
{
    public interface ILearnerService
    {
        Learner GetLearner(int learnerId, bool isLearnerArchived);
    }
}
