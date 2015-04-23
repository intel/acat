using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aster.WordPredictionManagement
{
    /// <summary>
    /// If the word predictor supports a settings dialog, this interface
    /// allows the WordPrediction manager to show it
    /// </summary>
    public interface IWordPredictorSettingsDialog
    {
        /// <summary>
        /// Gets a value to indicate whether the user save any settings
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Gets the Settings dialog form
        /// </summary>
        Form GetSettingsDialog { get; }
    }
}
