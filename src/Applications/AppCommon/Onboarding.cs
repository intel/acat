using ACAT.Core.PanelManagement;
using ACAT.Core.PreferencesManagement;
using ACAT.Core.PreferencesManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Extension;
using ACAT.Extensions.Onboarding.UI;
using ACAT.Extensions.Onboarding.UI.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ACAT.Applications
{
    public partial class AppCommon
    {
        public static bool DoOnboarding()
        {
            using (var onboardingForm = createOnboardingForm())
            {
                Application.Run(onboardingForm);
                return completeOnboarding(onboardingForm);
            }
        }

        public static async Task<bool> DoOnboardingAsync()
        {
            using (var onboardingForm = createOnboardingForm())
            {
                await ShowFormAsync(onboardingForm);
                return completeOnboarding(onboardingForm);
            }
        }

        private static bool completeOnboarding(OnboardingForm onboardingForm)
        {
            Context.AppActuatorManager.Dispose();
            return !onboardingForm.QuitOnboarding;
        }

        private static OnboardingForm createOnboardingForm()
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

            return new OnboardingForm
            {
                Sequence = onboardingSequence
            };

        }

        public static bool ResetAllPreferences(List<PreferencesCategory> currentCategory)
        {
            ILogger<AppCommon> logger = LoggingConfiguration.CreateLogger<AppCommon>();
            try
            {
                foreach (var preferences in resetPreferences(currentCategory))
                {
                    preferences.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error resetting preferences");
                return false;
            }
        }

        public static async Task<bool> ResetAllPreferencesAsync(List<PreferencesCategory> currentCategory)
        {
            ILogger<AppCommon> logger = LoggingConfiguration.CreateLogger<AppCommon>();
            try
            {
                foreach (var preferences in resetPreferences(currentCategory))
                {
                    // Preserve UI-affine synchronous extension preference contracts.
                    var saved = preferences is ACATPreferences acatPreferences
                        ? await acatPreferences.SaveAsync()
                        : preferences.Save();
                    if (!saved)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error resetting preferences");
                return false;
            }
        }

        private static IEnumerable<IPreferences> resetPreferences(List<PreferencesCategory> currentCategory)
        {
            if (CoreGlobals.AppPreferences != null)
            {
                CopyPreferencesValues(ACATPreferences.LoadDefaultSettings(), CoreGlobals.AppPreferences);
                yield return CoreGlobals.AppPreferences;
            }
            foreach (PreferencesCategory category in currentCategory)
            {
                if (category.PreferenceObj is ISupportsPreferences supportsPrefs)
                {
                    IPreferences defaultPrefs = supportsPrefs.GetDefaultPreferences();
                    IPreferences currentPrefs = supportsPrefs.GetPreferences();
                    if (defaultPrefs != null && currentPrefs != null)
                    {
                        CopyPreferencesValues(defaultPrefs, currentPrefs);
                        yield return currentPrefs;
                    }
                }
            }
        }

        private static void CopyPreferencesValues(IPreferences source, IPreferences target)
        {
            ILogger<AppCommon> logger = LoggingConfiguration.CreateLogger<AppCommon>();
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    try
                    {
                        var value = prop.GetValue(source);
                        PropertyInfo targetProp = targetType.GetProperty(prop.Name);

                        if (targetProp != null && targetProp.CanWrite)
                        {
                            targetProp.SetValue(target, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogDebug("Could not copy property {PropertyName}: {Message}", prop.Name, ex.Message);
                    }
                }
            }
        }
    }
}
