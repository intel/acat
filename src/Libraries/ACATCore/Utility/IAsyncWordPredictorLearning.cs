using ACAT.Core.WordPredictorManagement.Interfaces;
using System.Threading.Tasks;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Optional asynchronous learning contract for word predictors.
    /// </summary>
    public interface IAsyncWordPredictorLearning
    {
        Task<bool> LearnAsync(string text, WordPredictorMessageTypes requestType);
    }
}
