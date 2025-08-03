using ACAT.Core.PreferencesManagement;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Serializable class that represents info about the application
    /// to launch.  Includes name of the exe, command line arg, friendly name
    /// etc.
    /// </summary>
    [Serializable]
    public class AppInfo : PreferencesBase
    {
        /// <summary>
        /// To indicate a missing parameter
        /// </summary>
        [NonSerialized, XmlIgnore]
        private const string Missing = "";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AppInfo()
        {
            Name = String.Empty;
            Path = String.Empty;
            CommandLine = String.Empty;
        }

        ///// <summary>
        ///// Initializes a new instance of the class.
        ///// </summary>
        ///// <param name="appName">Friendly name of the app</param>
        ///// <param name="path">full path to the exe</param>
        ///// <param name="shortCutName">Name of the shortcut on desktop</param>
        ///// <param name="commandLine">optional command line arg</param>
        ///// <param name="description">user friendly description of the app</param>
        public AppInfo(String appName,
                        String path,
                        String shortCutName = Missing,
                        String commandLine = Missing,
                        String description = Missing,
                        String workingDirectory = Missing)
        {
            Name = appName;
            Path = path;
            ShortcutName = shortCutName;
            CommandLine = commandLine;
            Description = description;
            WorkingDirectory = workingDirectory;
        }

        /// <summary>
        /// Gets or sets the command line argument
        /// </summary>
        public String CommandLine { get; set; }

        /// <summary>
        /// Get or sets the description of the application
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets friendly name of the app
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the full path to the exe
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// Gets or sets the name of the shortcut
        /// </summary>
        public String ShortcutName { get; set; }

        /// <summary>
        /// Gets or sets the working directory for the shortcut
        /// </summary>
        public String WorkingDirectory { get; set; }
    }
}