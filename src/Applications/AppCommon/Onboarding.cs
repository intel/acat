using ACAT.Core.PanelManagement;
using ACAT.Extensions.Onboarding;
using System;
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

        public static bool ResetAllPreferences()
        {
            throw new NotImplementedException();
        }

        public static bool SaveAllPreferences()
        {
            throw new NotImplementedException();
        }
    }
}
