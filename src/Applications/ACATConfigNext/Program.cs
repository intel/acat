using System;
using System.Windows.Forms;

namespace ACATConfigNext
{
    internal static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new SettingsForm());
        }

        ///// <summary>
        /////  Handle different extension types separately  
        ///// </summary>
        ///// <param name="extension"></param>
        ///// <param name="supportsPrefs"></param>
        ///// <returns></returns>
        //private static TableLayoutPanel CreatePreferencesTableLayoutForExtension(IExtension extension, ISupportsPreferences supportsPrefs)
        //{
        //    switch (extension)
        //    {
        //        case IActuator actuator:
        //            return CreateActuatorPreferencesPanel(actuator, supportsPrefs);

        //        case ITTSEngine ttsEngine:
        //            return CreateTTSEnginePreferencesPanel(ttsEngine, supportsPrefs);

        //        case IWordPredictor wordPredictor:
        //            return CreateWordPredictorPreferencesPanel(wordPredictor, supportsPrefs);

        //        default:
        //            return CreateGenericExtensionPreferencesPanel(extension, supportsPrefs);
        //    }
        //}

        //private static TableLayoutPanel CreateActuatorPreferencesPanel(IActuator actuator, ISupportsPreferences supportsPrefs)
        //{
        //    var tableLayout = CustomControls.CreateCategoryTableLayoutPanel();

        //    var descriptor = actuator.Descriptor;
        //    tableLayout.Controls.Add(CustomControls.CreateLabel($"Actuator: {descriptor.Name}"));
        //    tableLayout.Controls.Add(CustomControls.CreateDescriptionLabel(descriptor.Description));

        //    var preferences = supportsPrefs.GetPreferences();
        //    if (preferences != null)
        //    {
        //        var props = preferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //        foreach (var prop in props)
        //        {
        //            var propPanel = CustomControls.CreateLabeledPanel(prop, preferences);
        //            var host = CustomControls.ElementHost(propPanel);
        //            tableLayout.Controls.Add(host);
        //        }
        //    }

        //    return tableLayout;
        //}

        //private static TableLayoutPanel CreateTTSEnginePreferencesPanel(ITTSEngine ttsEngine, ISupportsPreferences supportsPrefs)
        //{
        //    var tableLayout = CustomControls.CreateCategoryTableLayoutPanel();

        //    var descriptor = ttsEngine.Descriptor;
        //    tableLayout.Controls.Add(CustomControls.CreateLabel($"TTS Engine: {descriptor.Name}"));
        //    tableLayout.Controls.Add(CustomControls.CreateDescriptionLabel(descriptor.Description));

        //    var preferences = supportsPrefs.GetPreferences();
        //    if (preferences != null)
        //    {
        //        CreateTTSSpecificControls(tableLayout, preferences);
        //    }

        //    return tableLayout;
        //}

        //private static TableLayoutPanel CreateWordPredictorPreferencesPanel(IWordPredictor wordPredictor, ISupportsPreferences supportsPrefs)
        //{
        //    var tableLayout = CustomControls.CreateCategoryTableLayoutPanel();
        //    var descriptor = wordPredictor.Descriptor;
        //    tableLayout.Controls.Add(CustomControls.CreateLabel($"Word Predictor: {descriptor.Name}"));
        //    tableLayout.Controls.Add(CustomControls.CreateDescriptionLabel(descriptor.Description));

        //    var preferences = supportsPrefs.GetPreferences();
        //    if (preferences != null)
        //    {
        //        CreateWordPredictorSpecificControls(tableLayout, preferences);
        //    }

        //    return tableLayout;
        //}

        //private static TableLayoutPanel CreateGenericExtensionPreferencesPanel(IExtension extension, ISupportsPreferences supportsPrefs)
        //{
        //    var tableLayout = CustomControls.CreateCategoryTableLayoutPanel();

        //    var descriptor = extension.Descriptor;
        //    tableLayout.Controls.Add(CustomControls.CreateLabel($"Extension: {descriptor.Name}"));
        //    tableLayout.Controls.Add(CustomControls.CreateDescriptionLabel(descriptor.Description));

        //    var preferences = supportsPrefs.GetPreferences();
        //    if (preferences != null)
        //    {
        //        var props = preferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //        foreach (var prop in props)
        //        {
        //            var propPanel = CustomControls.CreateLabeledPanel(prop, preferences);
        //            var host = CustomControls.ElementHost(propPanel);
        //            tableLayout.Controls.Add(host);
        //        }
        //    }

        //    return tableLayout;
        //}

        //private static void CreateWordPredictorSpecificControls(TableLayoutPanel tableLayout, IPreferences preferences)
        //{
        //    var props = preferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    foreach (var prop in props)
        //    {
        //        switch (prop.Name.ToLower())
        //        {
        //            case "predictionwordcount":
        //                var wordCountPanel = CustomControls.CreateWordCountControl(prop, preferences);
        //                tableLayout.Controls.Add(wordCountPanel);
        //                break;

        //            case "ngram":
        //                var ngramPanel = CustomControls.CreateNGramControl(prop, preferences);
        //                tableLayout.Controls.Add(ngramPanel);
        //                break;

        //            case "filterpunctuationsenable":
        //                var punctuationPanel = CustomControls.CreatePunctuationFilterControl(prop, preferences);
        //                tableLayout.Controls.Add(punctuationPanel);
        //                break;

        //            case "supportslearning":
        //                var learningPanel = CustomControls.CreateLearningControl(prop, preferences);
        //                tableLayout.Controls.Add(learningPanel);
        //                break;

        //            case "filterchars":
        //                var filterCharsPanel = CustomControls.CreateFilterCharsControl(prop, preferences);
        //                tableLayout.Controls.Add(filterCharsPanel);
        //                break;

        //            case "usedefaultencoding":
        //                var encodingPanel = CustomControls.CreateEncodingControl(prop, preferences);
        //                tableLayout.Controls.Add(encodingPanel);
        //                break;

        //            case "showdisclaimeronStartup":
        //                var disclaimerPanel = CustomControls.CreateDisclaimerControl(prop, preferences);
        //                tableLayout.Controls.Add(disclaimerPanel);
        //                break;

        //            default:
        //                var propPanel = CustomControls.CreateLabeledPanel(prop, preferences);
        //                var host = CustomControls.ElementHost(propPanel);
        //                tableLayout.Controls.Add(host);
        //                break;
        //        }
        //    }
        //}

        //private static void CreateTTSSpecificControls(TableLayoutPanel tableLayout, IPreferences preferences)
        //{
        //    var props = preferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    foreach (var prop in props)
        //    {
        //        switch (prop.Name.ToLower())
        //        {
        //            case "voice":
        //                var voicePanel = CustomControls.CreateVoiceSelectionControl(prop, preferences);
        //                tableLayout.Controls.Add(voicePanel);
        //                break;

        //            case "rate":
        //                var ratePanel = CustomControls.CreateRateControl(prop, preferences);
        //                tableLayout.Controls.Add(ratePanel);
        //                break;

        //            case "volume":
        //                var volumePanel = CustomControls.CreateVolumeControl(prop, preferences);
        //                tableLayout.Controls.Add(volumePanel);
        //                break;

        //            case "pitch":
        //                var pitchPanel = CustomControls.CreatePitchControl(prop, preferences);
        //                tableLayout.Controls.Add(pitchPanel);
        //                break;

        //            default:
        //                var propPanel = CustomControls.CreateLabeledPanel(prop, preferences);
        //                var host = CustomControls.ElementHost(propPanel);
        //                tableLayout.Controls.Add(host);
        //                break;
        //        }
        //    }

        //}
    }
}