using ACAT.Core.PreferencesManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement;
using ACAT.Core.WordPredictorManagement.Interfaces;
using ACAT.Extension.UI;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace ACATExtension.Tests
{
    public class WordPredictorLearningTests
    {
        [Fact]
        public async Task LearnAsync_PrefersAsyncContractWithoutWaitingSynchronously()
        {
            var predictor = new AsyncPredictor();
            var result = WordPredictorLearning.LearnAsync(predictor, "phrase", WordPredictorMessageTypes.LearnCanned);

            Assert.Same(predictor.Completion.Task, result);
            Assert.False(result.IsCompleted);
            Assert.Equal(0, predictor.SyncCalls);
            Assert.Equal("phrase", predictor.Text);
            Assert.Equal(WordPredictorMessageTypes.LearnCanned, predictor.RequestType);

            predictor.Completion.SetResult(false);
            Assert.False(await result.ConfigureAwait(false));
        }

        [Fact]
        public async Task LearnAsync_FallsBackToLegacyContract()
        {
            var predictor = new LegacyPredictor();
            Assert.True(await WordPredictorLearning.LearnAsync(predictor, "words", WordPredictorMessageTypes.LearnWords).ConfigureAwait(false));
            Assert.Equal(1, predictor.SyncCalls);
            Assert.Equal("words", predictor.Text);
            Assert.Equal(WordPredictorMessageTypes.LearnWords, predictor.RequestType);
        }

        [Fact]
        public async Task LearnAsync_DoesNotCallPredictorWithoutLearningSupport()
        {
            var predictor = new AsyncPredictor { SupportsLearning = false };
            Assert.True(await WordPredictorLearning.LearnAsync(predictor, "phrase", WordPredictorMessageTypes.LearnCanned).ConfigureAwait(false));
            Assert.Null(predictor.Text);
            Assert.Equal(0, predictor.SyncCalls);
        }

        [Fact]
        public async Task LearnAsync_PropagatesAsyncFailure()
        {
            var predictor = new AsyncPredictor();
            var result = WordPredictorLearning.LearnAsync(predictor, "phrase", WordPredictorMessageTypes.LearnCanned);
            predictor.Completion.SetException(new InvalidOperationException("failed"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => result).ConfigureAwait(false);
        }

        [Fact]
        public void LearnAsync_RejectsMissingPredictor()
        {
            Assert.Throws<ArgumentNullException>(() =>
                WordPredictorLearning.LearnAsync(null, "phrase", WordPredictorMessageTypes.LearnCanned));
        }

        private class AsyncPredictor : LegacyPredictor, IAsyncWordPredictorLearning
        {
            public TaskCompletionSource<bool> Completion { get; } = new();

            public Task<bool> LearnAsync(string text, WordPredictorMessageTypes requestType)
            {
                Text = text;
                RequestType = requestType;
                return Completion.Task;
            }
        }

        private class LegacyPredictor : IWordPredictor
        {
            public int SyncCalls { get; private set; }
            public string Text { get; protected set; }
            public WordPredictorMessageTypes RequestType { get; protected set; }
            public bool SupportsLearning { get; set; } = true;
            public bool SupportsPredictSync => true;
            public bool SupportsPreferencesDialog => false;
            public bool FilterPunctuationsEnable { get; set; }
            public int NGram { get; set; }
            public int PredictionWordCount { get; set; }
            public Guid Id => Guid.Empty;
            public ClassDescriptorAttribute Descriptor => null;
            public event ModeChangedDelegate EvtModeChanged { add { } remove { } }
            public event NextLetterProbabilitiesDelegate EvtNotifyNextLetterProbabilities { add { } remove { } }
            public event NextWordProbabilitiesDelegate EvtNotifyNextWordProbabilities { add { } remove { } }
            public event WordPredictionAsyncResponseDelegate EvtWordPredictionAsyncResponse { add { } remove { } }

            public bool Learn(string text, WordPredictorMessageTypes requestType)
            {
                SyncCalls++;
                Text = text;
                RequestType = requestType;
                return true;
            }

            public void Dispose() { }
            public ExtensionInvoker GetInvoker() => null;
            public IPreferences GetDefaultPreferences() => null;
            public IPreferences GetPreferences() => null;
            public WordPredictionModes GetMode() => WordPredictionModes.None;
            public bool Init(CultureInfo ci) => true;
            public bool PostInit() => true;
            public int LoadContext(string text) => -1;
            public bool LoadDefaultSettings() => true;
            public bool LoadSettings(string configFileDirectory) => true;
            public WordPredictionResponse Predict(WordPredictionRequest req) => null;
            public bool PredictAsync(WordPredictionRequest req) => false;
            public bool SaveSettings(string configFileDirectory) => true;
            public void SetMode(WordPredictionModes wordPredictionMode) { }
            public bool ShowPreferencesDialog() => false;
            public void UnloadContext(int contextHandle) { }
        }
    }
}
