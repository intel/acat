using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aster.Utility;

namespace Aster.WordPredictionManagement
{
    /// <summary>
    /// Interface to Word prediction engines.  A word predictor predicts 
    /// the next word using the previous n-words as context.  Some word predictors
    /// also support learning where the engine tunes its internal model for 
    /// more accurate predictions based on the user.
    /// </summary>
    public interface IWordPredictor : IDisposable
    {
        /// <summary>
        /// Initialize the word predictor.
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        bool Init();

        /// <summary>
        /// Returns a descriptor which contains a user readable name, a 
        /// short textual description and a unique GUID.
        /// </summary>
        IDescriptor Descriptor { get; }

        /// <summary>
        /// Returns a list of next word predictions based on the context
        /// from the previous words in the sentence.  The number of words
        /// returned is set by the PredictionWordCount property
        /// </summary>
        /// <param name="prevNWords">Previous words in the sentence</param>
        /// <param name="lastWord">current word (may be partially spelt out</param>
        /// <returns>A list of predicted words</returns>
        IEnumerable<String> Predict(String prevNWords, String lastWord);

        /// <summary>
        /// Returns a flag indicating whether the word predictor supports
        /// dynamic learning.
        /// </summary>
        bool SupportsLearning { get; }

        /// <summary>
        /// Add the text to the word prediction models for better
        /// prediction.  The text would typically represent something
        /// the user typed.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool Learn(String text);

        /// <summary>
        /// Sets the number of predicted words to return.
        /// </summary>
        int PredictionWordCount { get; set; }

        /// <summary>
        /// Whether puncutations should be a part of the prediction. For 
        /// instance, some word predictors may return a question mark as a
        /// prediction if it detects the end of a sentence.
        /// </summary>
        bool FilterPunctuationsEnable { get; set; }

        /// <summary>
        /// How many previous words to use for prediction.
        /// </summary>
        int NGram { get; set; }

        /// <summary>
        /// Load settings from a file maintained by the word predictor.
        /// </summary>
        /// <param name="configFileDirectory">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        bool LoadSettings(String configFileDirectory);

        /// <summary>
        /// Reset to factory default settings
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        bool LoadDefaultSettings();

        /// <summary>
        /// Save the word predictor settings to a file that is maintained
        /// by the word predictor.
        /// </summary>
        /// <param name="configFileDirectory">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        bool SaveSettings(String configFileDirectory);

        /// <summary>
        /// Returns a flag to indicate whether the word predictor supports a settings 
        /// dialog.  true if it does, false if it doesn't
        /// </summary>
        bool SupportsSettingsDialog { get; }

        /// <summary>
        /// Returns a dialog interface that the application will display to the user
        /// to change settings that are specific to the word predictor
        /// </summary>
        /// <returns>The setttings dialog</returns>
        IWordPredictorSettingsDialog SettingsDialog {get; }

        /// <summary>
        /// Call this on a context switch to a document.  Creates
        /// a temporary model
        /// </summary>
        /// <param name="text">Text from the document</param>
        /// <returns>Handle.  Pass this to unload</returns>
        int LoadContext(String text);

        /// <summary>
        /// Unload context
        /// </summary>
        /// <param name="contextHandle">Handle returned by load</param>
        void UnloadContext(int contextHandle);
    }
}
