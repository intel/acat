using ACAT.Core.ActuatorManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PreferencesManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Extension;
using ACAT.Extensions.Onboarding;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Applications
{
    public partial class AppCommon
    {
        public static bool DoOnboarding()
        {
            Guid welcome = new("6d8da00e-5035-4b7f-a646-ed9f840a13bf");
            Guid languageSelect = new("{F2803F8A-D639-459C-9F27-5742BAD4E405}");
            Guid switchSelect = new("301dbc87-c98c-491a-a2ee-d17863eab831");
            Guid keyboardConfig = new("65b95de3-bf5a-4ae8-b44d-f5e7950ab8d6");
            Guid finish = new("e03754b3-85af-4f43-855e-47e20f7400c2");

            var onboardingSequence = new OnboardingSequence();

            onboardingSequence.OnboardingSequenceItems.Add(new OnboardingSequenceItem(welcome));
            onboardingSequence.OnboardingSequenceItems.Add(new OnboardingSequenceItem(languageSelect));
            onboardingSequence.OnboardingSequenceItems.Add(new OnboardingSequenceItem(switchSelect));
            onboardingSequence.OnboardingSequenceItems.Add(new OnboardingSequenceItem(keyboardConfig));
            onboardingSequence.OnboardingSequenceItems.Add(new OnboardingSequenceItem(finish));

            var onboardingForm = new OnboardingForm
            {
                Sequence = onboardingSequence
            };

            Application.Run(onboardingForm);

            Context.AppActuatorManager.Dispose();

            if (onboardingForm.QuitOnboarding)
            {
                return false;
            }

            return true;
        }

        public static bool ResetAllPreferences(List<PreferencesCategory> currentCategory)
        {
            try
            {
                // Reset general preferences  
                if (CoreGlobals.AppPreferences != null)
                {
                    var defaultPrefs = ACATPreferences.LoadDefaultSettings();
                    if (defaultPrefs != null)
                    {
                        // Copy default values to current preferences  
                        CopyPreferencesValues(defaultPrefs, CoreGlobals.AppPreferences);
                        CoreGlobals.AppPreferences.Save();
                    }
                }

                foreach (var category in currentCategory)
                {
                    if (category.PreferenceObj is ISupportsPreferences supportsPrefs)
                    {
                        var defaultPrefs = supportsPrefs.GetDefaultPreferences();
                        var currentPrefs = supportsPrefs.GetPreferences();

                        if (defaultPrefs != null && currentPrefs != null)
                        {
                            CopyPreferencesValues(defaultPrefs, currentPrefs);
                            currentPrefs.Save();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
        }

        private static void CopyPreferencesValues(IPreferences source, IPreferences target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();

            var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    try
                    {
                        var value = prop.GetValue(source);
                        var targetProp = targetType.GetProperty(prop.Name);

                        if (targetProp != null && targetProp.CanWrite)
                        {
                            targetProp.SetValue(target, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug($"Could not copy property {prop.Name}: {ex.Message}");
                    }
                }
            }
        }
    }
}
