using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.SpellCheckManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Lib.Core.DialogSenseManagement
{
    [DescriptorAttribute("B5BDE0BC-4DD6-4F0D-81ED-38B074D7FE85",
                       "Null DialogSense Agent",
                       "No dialog sensing functionality.")]
    internal class NullDialogSenseAgent : IDialogSenseAgent, IExtension
    {
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        private DialogTranscript _dialogTranscript;

        public NullDialogSenseAgent()
        {
            _dialogTranscript = new DialogTranscript();
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        public event MessageReceivedEventHandler EvtMessageReceived;

        public async Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            return;
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the invoker object
        /// </summary>
        /// <returns>Invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return null;
        }

        /// <summary>
        /// Performs initialization
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>true</returns>
        public bool Init(CultureInfo ci)
        {
            return true;
        }

        /// <summary>
        /// Performs initialization
        /// </summary>
        /// <returns>true onf success</returns>
        public bool Init()
        {
            return true;
        }

        public bool IsConnected()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load factory default settings.
        /// </summary>
        /// <returns>true on success</returns>
        public bool LoadDefaultSettings()
        {
            return true;
        }

        /// <summary>
        /// Loads settings from the specified directory
        /// </summary>
        /// <param name="configFileDirectory">Location of the config file</param>
        /// <returns>true on success</returns>
        public bool LoadSettings(String configFileDirectory)
        {
            return true;
        }

        /// <summary>
        /// Looks up the specified word and returns the
        /// correct spelling
        /// </summary>
        /// <param name="word">word to lookup</param>
        /// <returns>spelling</returns>
        public String Lookup(String word)
        {
            return String.Empty;
        }

        /// <summary>
        /// Saves the settings (no-op)
        /// </summary>
        /// <param name="configFileDirectory">file</param>
        /// <returns></returns>
        public bool SaveSettings(String configFileDirectory)
        {
            return true;
        }

        public Task SendAsync(string jsonMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Uninitializes
        /// </summary>
        public void Uninit()
        {
        }

        DialogSenseSource GetDialogSenseSource 
        { 
            get
            {
                return DialogSenseSource.None;
            }
        }

        DialogSenseSource IDialogSenseAgent.GetDialogSenseSource => DialogSenseSource.None;

        public DialogTranscript DialogTranscript => _dialogTranscript;

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }
    }
}
