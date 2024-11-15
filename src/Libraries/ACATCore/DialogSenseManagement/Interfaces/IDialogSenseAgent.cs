using ACAT.Lib.Core.Utility;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace ACAT.Lib.Core.DialogSenseManagement
{
    public enum DialogSenseSource
    {
        None = 0,
        ASR = 1
    }

    public delegate void JsonMessageReceivedEventHandler(object sender, String message);

    public delegate void MessageReceivedEventHandler(object sender, String message);


    public interface IDialogSenseAgent : IDisposable
    {
        /// <summary>
        /// Returns a descriptor which contains a user readable name, a
        /// short textual description and a unique GUID.
        /// </summary>
        IDescriptor Descriptor { get; }

        /// <summary>
        /// Initialize the spell checker
        /// </summary>
        /// <param name="ci">Language for the spellchecker</param>
        /// <returns>true on success, false on failure</returns>
        bool Init(CultureInfo ci);

        /// <summary>
        /// Reset to factory default settings
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        bool LoadDefaultSettings();

        /// <summary>
        /// Load settings from a file maintained by the word predictor.
        /// </summary>
        /// <param name="configFileDirectory">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        bool LoadSettings(String configFileDirectory);

        /// <summary>
        /// Save the word predictor settings to a file that is maintained
        /// by the word predictor.
        /// </summary>
        /// <param name="configFileDirectory">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        bool SaveSettings(String configFileDirectory);

        Task ConnectAsync();

        void Disconnect();

        Task SendAsync(String jsonMessage);

        bool IsConnected();

        DialogSenseSource GetDialogSenseSource { get; }

        /// <summary>
        /// Uninitializes
        /// </summary>
        void Uninit();

        event JsonMessageReceivedEventHandler EvtJsonMessageReceived;

        event MessageReceivedEventHandler EvtMessageReceived;


        DialogTranscript DialogTranscript { get; }


    }
}