using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace ACAT.Extensions.FunctionalAgents.LaunchAppAgent
{
    /// <summary>
    /// Serializable class that represents info about the application
    /// to launch.  Includes name of the exe, command line arg, friendly name
    /// etc.
    /// </summary>
    [Serializable]
    public class AppInfo
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
            Action = LaunchAction.StartNew;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="appName">Friendly name of the app</param>
        /// <param name="path">full path to the exe</param>
        /// <param name="commandLine">optional command line arg</param>
        /// <param name="action">start a new instance or switch to existing</param>
        public AppInfo(String appName, String path, String commandLine = Missing, LaunchAction action = LaunchAction.StartNew)
        {
            Name = appName;
            Path = path;
            CommandLine = commandLine;
            Action = action;
        }

        /// <summary>
        /// How to launch?
        /// </summary>
        public enum LaunchAction
        {
            /// <summary>
            /// No action
            /// </summary>
            None,

            /// <summary>
            /// Starts a new instance of the application
            /// </summary>
            StartNew,

            /// <summary>
            /// Switches to an existing instance of the application
            /// </summary>
            SwitchTo
        }

        /// <summary>
        /// Gets or sets the launch action
        /// </summary>
        public LaunchAction Action { get; set; }

        /// <summary>
        /// Gets or sets the command line argument
        /// </summary>
        public String CommandLine { get; set; }

        /// <summary>
        /// Gets or sets friendly name of the app
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the full path to the exe
        /// </summary>
        public String Path { get; set; }
    }
}