using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement.Interfaces;
using System;
using System.Threading.Tasks;

namespace ACAT.Extension.UI
{
    /// <summary>
    /// Selects task-based learning when supported without changing the synchronous predictor contract.
    /// </summary>
    public static class WordPredictorLearning
    {
        public static Task<bool> LearnAsync(IWordPredictor predictor, string text, WordPredictorMessageTypes requestType)
        {
            if (predictor == null)
            {
                throw new ArgumentNullException(nameof(predictor));
            }
            if (!predictor.SupportsLearning)
            {
                return Task.FromResult(true);
            }
            if (predictor is IAsyncWordPredictorLearning asyncPredictor)
            {
                return asyncPredictor.LearnAsync(text, requestType);
            }
            return Task.FromResult(predictor.Learn(text, requestType));
        }
    }
}
