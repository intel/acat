////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TTSClient.cs
//
// Converts text to speech by sending the text string to a TTS server using
// the supported protocols
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ACAT.Extensions.Default.TTSEngines.TTSClient
{
    /// <summary>
    /// Converts text to speech by sending the text string to a TTS server using
    /// the supported protocols
    /// </summary>
    ///
    [DescriptorAttribute("33A07974-72A5-4147-A8EA-7B001520C175",
                        "TTS Client",
                        "Text to Speech client that sends the text to be converted to a server using the supported protocols")]
    public class TTSClient : ExtensionInvoker, ITTSEngine, ISupportsPreferences
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        internal const String SettingsFileName = "TTSClientSettings.xml";

        /// <summary>
        /// Settings for this engine
        /// </summary>
        internal static TTSClientSettings Settings;

        private const int MaxPitch = int.MaxValue;

        /// <summary>
        /// Max value of rate of speech
        /// </summary>
        private const int MaxRate = int.MaxValue;

        /// <summary>
        /// Max value of speech volume
        /// </summary>
        private const int MaxVolume = int.MaxValue;

        private const int MinPitch = int.MinValue;

        /// <summary>
        /// Minimum value of rate of speech
        /// </summary>
        private const int MinRate = int.MinValue;

        /// <summary>
        /// Minimum value of speech volume
        /// </summary>
        private const int MinVolume = 0;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Is speech muted?
        /// </summary>
        private bool _muted;

        /// <summary>
        /// Next value of the bookmark to generate
        /// </summary>
        private int _nextBookmark = 1;

        /// <summary>
        /// Pitch setting
        /// </summary>
        private int _pitch;

        /// <summary>
        /// Volume level before Mute was set
        /// </summary>
        private int _preMuteVolumeLevel;

        /// <summary>
        /// The alternate pronunciations file
        /// </summary>
        private Pronunciations _pronunciations;

        /// <summary>
        /// Rate setting
        /// </summary>
        private int _rate;

        private Transport _transport;

        /// <summary>
        /// Volume setting
        /// </summary>
        private int _volume;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TTSClient()
        {
            TTSClientSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            Settings = TTSClientSettings.Load();
            UseAlternatePronunciations = Settings.UseAlternatePronunciations;
            _transport = new Transport();
        }

#pragma warning disable

        /// <summary>
        /// Triggered when bookmark reached after async text to speech
        /// </summary>
        public event TTSBookmarkReached EvtBookmarkReached;

        /// <summary>
        /// Raised when one of the properties changes
        /// </summary>
        public event EventHandler EvtPropertyChanged;

        /// <summary>
        /// Triggered when the status of the speech engine changes
        /// </summary>
        public event TTSStatusChanged EvtStatusChanged;

        /// <summary>
        /// Triggered when voice is changed
        /// </summary>
        public event TTSVoiceChanged EvtVoiceChanged;

#pragma warning enable

        /// <summary>
        /// Gets or sets the culture info for the voice to use
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Returns the current status of the speech engine, whether
        /// it is currently speaking or paused or something else
        /// </summary>
        public StatusFlags Status
        {
            get
            {
                return StatusFlags.None;
            }
        }

        /// <summary>
        /// Gets whether this supports a custom settings dialog
        /// </summary>
        public virtual bool SupportsPreferencesDialog
        {
            get { return false; }
        }

        /// <summary>
        /// Get/sets whether alternate pronunciations should be used or not
        /// before TTS
        /// </summary>
        public bool UseAlternatePronunciations { get; set; }

        /// <summary>
        /// Gets or sets the voice to use. Not supported
        /// </summary>
        public String Voice
        {
            get { return String.Empty; }

            set { }
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
        /// Returns the default preferences (factory settings)
        /// </summary>
        /// <returns>Default preferences object</returns>
        public IPreferences GetDefaultPreferences()
        {
            return TTSClientSettings.LoadDefaults<TTSClientSettings>();
        }

        /// <summary>
        /// Pitch of speech.  Value is engine-dependent. SAPI
        /// doesn't support it
        /// </summary>
        public TTSValue GetPitch()
        {
            return new TTSValue(MinPitch, MaxPitch, _pitch);
        }

        /// <summary>
        /// Returns the preferences object
        /// </summary>
        /// <returns>The preferences object</returns>
        public IPreferences GetPreferences()
        {
            return Settings;
        }

        /// <summary>
        /// Rate of speech.  Value is engine-dependent
        /// </summary>
        public TTSValue GetRate()
        {
            return new TTSValue(MinRate, MaxRate, _rate);
        }

        /// <summary>
        /// Returns a list of voices supported by the speech engine
        /// </summary>
        /// <returns>List of names of vlices</returns>
        public List<String> GetVoices()
        {
            return new List<String>();
        }

        /// <summary>
        /// Volume of speech. Value is engine-dependent
        /// </summary>
        public TTSValue GetVolume()
        {
            return new TTSValue(MinVolume,
                                MaxVolume,
                                IsMuted() ? _preMuteVolumeLevel : _volume);
        }

        /// <summary>
        /// Call this first.  Initializes the speech engine
        /// </summary>
        /// <returns>true on success</returns>
        public bool Init(CultureInfo ci)
        {
            _volume = Settings.Volume;
            _pitch = Settings.Pitch;
            _rate = Settings.Rate;

            loadPronunciations(ci);

            return true;
        }

        /// <summary>
        /// Returns true if muted
        /// </summary>
        /// <returns>true if muted</returns>
        public bool IsMuted()
        {
            return _muted;
        }

        /// <summary>
        /// Mutes the speech engine (volume set to 0)
        /// </summary>
        public void Mute()
        {
            if (!IsMuted())
            {
                _preMuteVolumeLevel = GetVolume().Value;
                _muted = true;

                notifyPropertyChanged();
            }
        }

        /// <summary>
        /// Pauses the speech
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public bool Pause()
        {
            return true;
        }

        /// <summary>
        /// Restores default settings
        /// </summary>
        public void RestoreDefaults()
        {
            var settings = new TTSClientSettings();

            _rate = settings.Rate;
            _volume = settings.Volume;
        }

        /// <summary>
        /// Resumes paused speech
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public bool Resume()
        {
            return true;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public bool Save()
        {
            Settings.Volume = _volume;
            Settings.Rate = _rate;
            Settings.Pitch = _pitch;

            Settings.Save();

            return true;
        }

        /// <summary>
        /// Sets pitch. SAPI doesn't support it.
        /// </summary>
        /// <param name="pitch">value</param>
        /// <returns>false</returns>
        public bool SetPitch(int pitch)
        {
            _pitch = pitch;

            notifyPropertyChanged();

            return true;
        }

        /// <summary>
        /// Sets the rate of speech. Value is engine dependent
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public bool SetRate(int rate)
        {
            _rate = rate;

            notifyPropertyChanged();

            return true;
        }

        /// <summary>
        /// Sets the volume level. Value is synth specific
        /// </summary>
        /// <param name="volume">volume level</param>
        /// <returns>true on success</returns>
        public bool SetVolume(int volume)
        {
            if (volume == 0)
            {
                Mute();
            }
            else
            {
                _muted = false;
                _volume = volume;
                notifyPropertyChanged();
            }

            return true;
        }

        /// <summary>
        /// Shows the preferences dialog
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool ShowPreferencesDialog()
        {
            return false;
        }

        /// <summary>
        /// Convert the string to speech by sending it to the
        /// speech synthesizer.  Conversion is done asynchronously.
        /// So the function returns immediately.
        /// </summary>
        /// <param name="text">Text to convert.  Can have multiple sentences</param>
        /// <returns>true on success</returns>
        public bool Speak(String text)
        {
            Log.Debug("Entering...text=" + text);

            try
            {
                if (!IsMuted())
                {
                    sendText(autoAppendPunctuation(replaceWithAltPronunciations(text)));
                }
            }
            catch (Exception ex)
            {
                Log.Debug("exception caught! ex=" + ex.Message);
            }

            return true;
        }

        /// <summary>
        /// Sends the string to the speech syncth to speak
        /// async. Returns a bookmark. When the bookmark is
        /// reached, an event raised.
        /// </summary>
        /// <param name="text">String to convert</param>
        /// <param name="bookmark">returns bookmark</param>
        /// <returns>true on success</returns>
        public bool SpeakAsync(String text, out int bookmark)
        {
            bool retVal = true;

            bookmark = _nextBookmark++;
            try
            {
                if (!IsMuted())
                {
                    sendText(autoAppendPunctuation(replaceWithAltPronunciations(text)));
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Convert the string to speech by sending it to the
        /// speech synthesizer.  Conversion is done asynchronously.
        /// So the function returns immediately.
        /// </summary>
        /// <param name="ssml">Text to convert (SSML format)</param>
        /// <returns>true on success</returns>
        public bool SpeakSsml(String ssml, String text, String placeHolder)
        {
            Log.Debug("Entering...text=" + ssml);

            try
            {
                if (!IsMuted())
                {
                    text = autoAppendPunctuation(replaceWithAltPronunciations(text));
                    ssml = ssml.Replace(placeHolder, text);

                    Log.Debug("Speaking text:" + text);
                    Log.Debug("ssml:" + ssml);

                    sendSsml(ssml);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("exception caught! ex=" + ex.Message);
            }

            return true;
        }

        /// <summary>
        /// Sends the string to the speech syncth to speak
        /// async. Returns a bookmark. When the bookmark is
        /// reached, an event raised.
        /// </summary>
        /// <param name="ssml">String to convert (SSML format)</param>
        /// <param name="bookmark">returns bookmark</param>
        /// <returns>true on success</returns>
        public bool SpeakSsmlAsync(String ssml, String text, String placeHolder, out int bookmark)
        {
            bool retVal = true;

            bookmark = _nextBookmark++;
            try
            {
                if (!IsMuted())
                {
                    text = autoAppendPunctuation(replaceWithAltPronunciations(text));
                    ssml = ssml.Replace(placeHolder, text);

                    Log.Debug("Speaking text:" + text);
                    Log.Debug("ssml:" + ssml);

                    sendSsml(ssml);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Stops the speech. Cancels all async operations in
        /// progress
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public bool Stop()
        {
            return false;
        }

        /// <summary>
        /// Unmutes the speech engine. Restore previous
        /// volume
        /// </summary>
        public void UnMute()
        {
            if (IsMuted())
            {
                _muted = false;
                _volume = _preMuteVolumeLevel;
                notifyPropertyChanged();
            }
        }

        /// <summary>
        /// Waits until speech finishes or timeout expires
        /// </summary>
        /// <param name="timeout">Timeout to wait in milliseconds</param>
        /// <returns>true on success, false on failure</returns>
        public bool WaitUntilDone(int timeout)
        {
            // Synthesizer.WaitUntilDone(timeout);

            return true;
        }

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

        /// <summary>
        /// If there is no puncuation at the end, appends it, if the settings
        /// parameter is true
        /// </summary>
        /// <param name="inputString">input string</param>
        /// <returns>output string</returns>
        private String autoAppendPunctuation(String inputString)
        {
            inputString = inputString.Trim();
            if (!Settings.AutoAppendPunctuation || inputString.Length == 0)
            {
                return inputString;
            }

            char ch = inputString[inputString.Length - 1];
            if (TextUtils.IsSentenceTerminator(ch))
            {
                return inputString;
            }

            /// TODO:  The period should be language sensitive
            return inputString + ".";
        }

        private String getTempFileName(String extension)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + "\\Temp";
            String fileName = Guid.NewGuid().ToString() + extension;

            try
            {
                if (!Directory.Exists(path))
                {
                    if (Directory.CreateDirectory(path) == null)
                    {
                        path = ".\\";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Could not create temp directory for TTSClient. " + ex);
                path = ".\\";
            }

            path = path + "\\TTS_" + Guid.NewGuid() + extension;
            path = path.Replace("\\", "/");

            return path;
        }

        /// <summary>
        /// Reads alternate pronunciations from the pronunciations file
        /// </summary>
        /// <returns>true on success</returns>
        private bool loadPronunciations(CultureInfo ci)
        {
            if (_pronunciations != null)
            {
                _pronunciations.Dispose();
            }

            _pronunciations = new Pronunciations();

            Log.Debug("Loading pronunciations. Filename is " + Settings.PronunciationsFile);
            return _pronunciations.Load(ci, Settings.PronunciationsFile);
        }

        /// <summary>
        /// Notifies subscribes that the bookmark was reached
        /// </summary>
        /// <param name="bookmark">the bookmark</param>
        private void notifyBookmarkReached(int bookmark)
        {
            if (EvtBookmarkReached != null)
            {
                EvtBookmarkReached(this, new TTSBookmarkReachedEventArgs(bookmark));
            }
        }

        /// <summary>
        /// Notifies that property changed
        /// </summary>
        private void notifyPropertyChanged()
        {
            if (EvtPropertyChanged != null)
            {
                EvtPropertyChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Parses the input string and replaces all the words
        /// that have alternate pronunciations
        /// </summary>
        /// <param name="inputString">input string</param>
        /// <returns>string with alternate pronunciations</returns>
        private String replaceWithAltPronunciations(String inputString)
        {
            return UseAlternatePronunciations ?
                    _pronunciations.ReplaceWithAlternatePronunciations(inputString) :
                    inputString;
        }

        private bool sendSsml(String ssml)
        {
            bool retVal = true;
            String data;

            switch (_transport.Format)
            {
                case TTSFormat.SSML:
                    data = ssml;
                    break;

                case TTSFormat.Json:
                    data = ssmlToJson(ssml);
                    break;

                default:
                case TTSFormat.Text:
                    return false;
            }

            return _transport.Send(data, _transport.Format);
        }

        private bool sendText(String text)
        {
            bool retVal = true;
            String data;

            switch (_transport.Format)
            {
                case TTSFormat.SSML:
                    data = textToSsml(text);
                    break;

                case TTSFormat.Json:
                    data = textToJson(text);
                    break;

                default:
                case TTSFormat.Text:
                    data = text;
                    break;
            }

            return _transport.Send(data, _transport.Format);
        }

        private String ssmlFileToJson(String fileName)
        {
            String requestID = Guid.NewGuid().ToString();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("\"source\":\"acat\",");
            sb.AppendLine("\"requestID\":\"" + requestID + "\",");
            sb.AppendLine("\"requestType\":\"tts\",");
            sb.AppendLine("\"params\":");
            sb.AppendLine("{");
            sb.AppendLine("\"name\":\"ttsText\", ");
            sb.AppendLine("\"valueRef\":\"file\",");
            sb.AppendLine("\"valueFormat\":\"xml\",");
            sb.AppendLine("\"value\":\"" + fileName + "\"");
            sb.AppendLine("}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private String ssmlToJson(String ssml)
        {
            var tempFile = getTempFileName(".xml");
            writeToFile(tempFile, ssml);

            return ssmlFileToJson(tempFile);
        }

        /// <summary>
        /// Converts text to Json format
        /// </summary>
        /// <param name="text">Text to convert</param>
        /// <returns>Json string</returns>
        private String textToJson(String text)
        {
            var ssml = textToSsml(text);
            var tempFile = getTempFileName(".xml");
            writeToFile(tempFile, ssml);

            return ssmlFileToJson(tempFile);
        }

        /// <summary>
        /// Converts text to ssml
        /// </summary>
        /// <param name="text">Text to convert</param>
        /// <returns>SSML fragment</returns>
        private String textToSsml(String text)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
            sb.AppendLine("<speak version=\"1.1\" xmlns=\"http://www.w3.org/2001/10/synthesis\"");
            //sb.AppendLine("xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\"");
            //sb.AppendLine("xsi:schemaLocation=\"http://www.w3.org/TR/speech-synthesis11/synthesis.xsd\">");
            sb.AppendLine("xml:lang=\"" + CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName + "\">");
            sb.AppendLine("<p><s>");
            sb.AppendLine(text);
            sb.AppendLine("</s></p>");
            sb.AppendLine("</speak>");

            return sb.ToString();
        }

        private bool writeToFile(String fileName, String text)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(fileName))
                {
                    file.WriteLine(text);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error writing to temp file " + fileName + ". Exception: " + ex);
                return false;
            }

            return true;
        }
    }
}