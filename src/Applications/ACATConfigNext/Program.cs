using ACAT.Core.Utility;
using ACAT.Core.PanelManagement;
using ACATConfigNext.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using ACAT.Applications;
using System.Threading.Tasks;

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
            AppCommon.RunApplication(StartApplicationAsync);
        }

        private static async Task StartApplicationAsync()
        {
            await AppCommon.LoadGlobalSettingsAsync();
            await AppCommon.SetUserNameAsync();
            AppCommon.SetProfileName();
            if (!AppCommon.CreateUserAndProfile() ||
                !await AppCommon.LoadUserPreferencesAsync())
            {
                return;
            }
            AppCommon.SetCulture();

            // Set up dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            Context.ServiceProvider = serviceProvider;

            // Get SettingsForm from DI container
            using SettingsForm settingsForm = serviceProvider.GetRequiredService<SettingsForm>();
            await AppCommon.ShowFormAsync(settingsForm);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Add ACAT infrastructure (logging and core services)
            services.AddACATInfrastructure();

            // Register SettingsForm - IServiceProvider will be injected automatically
            services.AddTransient(provider => new SettingsForm(
                provider.GetRequiredService<ILogger<SettingsForm>>(), provider, preferencesLoaded: true));
        }
    }
}