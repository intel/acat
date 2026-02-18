using ACATConfigNext.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            // Set up dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            // Get SettingsForm from DI container
            SettingsForm settingsForm = serviceProvider.GetRequiredService<SettingsForm>();
            Application.Run(settingsForm);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Add ACAT infrastructure (logging and core services)
            services.AddACATInfrastructure();

            // Register SettingsForm - IServiceProvider will be injected automatically
            services.AddTransient<SettingsForm>();
        }
    }
}