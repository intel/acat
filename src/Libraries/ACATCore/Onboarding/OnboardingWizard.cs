////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Onboarding
{
    /// <summary>
    /// Represents the onboarding wizard controller. Controls navigation
    /// between the extensions and between the steps in each extension
    /// </summary>
    public class OnboardingWizard : IOnboardingWizard
    {
        private static volatile bool _DLLError = false;
        private int _extensionIndex = -1;
        private List<Type> _extensionsTypeCache = new List<Type>();
        private List<OnboardingHistoryEntry> _history = new List<OnboardingHistoryEntry>();
        private List<IOnboardingExtension> _onboardingExtensions = new List<IOnboardingExtension>();
        private OnboardingSequence _onboardingSequence;

        public OnboardingWizard()
        {
        }

        public delegate void AddCustomButtonDelegate(Control control, OnboardingButtonTypes buttonType);

        public delegate void GoBackDelegate(IOnboardingExtension source);

        public delegate void GotoNextDelegate(IOnboardingExtension source);

        public delegate void QuitDelegate(IOnboardingExtension source, Reason reason, bool confirm);

        public delegate void SetButtonEnabledDelegate(OnboardingButtonTypes button, bool enable);

        public delegate void SetButtonTextDelegate(OnboardingButtonTypes button, String text);

        public delegate void SetButtonVisibleDelegate(OnboardingButtonTypes button, bool visible);

        public event AddCustomButtonDelegate EvtAddCustomButton;

        public event GoBackDelegate EvtGoBack;

        public event GotoNextDelegate EvtGotoNext;

        public event QuitDelegate EvtQuit;

        public event SetButtonEnabledDelegate EvtSetButtonEnabled;

        public event SetButtonTextDelegate EvtSetButtonText;

        public event SetButtonVisibleDelegate EvtSetButtonVisible;

        public void AddCustomButton(Control control, OnboardingButtonTypes buttonType)
        {
            if (EvtAddCustomButton != null)
            {
                EvtAddCustomButton(control, buttonType);
            }
        }

        public void AddToHistory(IOnboardingExtension obe, String step)
        {
            _history.Add(new OnboardingHistoryEntry(obe, step));
        }

        public IOnboardingExtension GetNextOnboardingExtension()
        {
            if ((_extensionIndex + 1) >= _onboardingExtensions.Count)
            {
                return null;
            }

            _extensionIndex++;

            return _onboardingExtensions[_extensionIndex];
        }

        public OnboardingHistoryEntry GetPrevious()
        {
            if (_history.Count < 2)
            {
                return null;
            }

            var currentEntry = _history[_history.Count - 1];

            _history.RemoveAt(_history.Count - 1);

            var prevEntry = _history[_history.Count - 1];

            if (prevEntry.OnboardingExtension != currentEntry.OnboardingExtension)
            {
                if (prevEntry.OnboardingExtension.StartOverOnBackwardNavigation)
                {
                    var savEntry = prevEntry;

                    while (true)
                    {
                        if (savEntry.OnboardingExtension != prevEntry.OnboardingExtension)
                        {
                            prevEntry = savEntry;
                            break;
                        }

                        savEntry = _history[_history.Count - 1];

                        _history.RemoveAt(_history.Count - 1);

                        if (_history.Count == 0)
                        {
                            break;
                        }

                        prevEntry = _history[_history.Count - 1];
                    }
                }
            }

            for (int i = _history.Count - 1; i >= 0; i--)
            {
                int index = findOnboardingExtension(_history[i].OnboardingExtension);
                if (index >= 0)
                {
                    _extensionIndex = index;
                    break;
                }
            }

            return prevEntry;
        }

        public void GoBack(IOnboardingExtension source)
        {
            EvtGoBack?.Invoke(source);
        }

        public void GotoNext(IOnboardingExtension source)
        {
            EvtGotoNext?.Invoke(source);
        }

        public bool Initialize(OnboardingSequence sequence)
        {
            //OnboardingSequence.SettingsFilePath = "OnboardingSequence.xml";

            //_onboardingSequence = OnboardingSequence.Load();

            _onboardingSequence = sequence;

            loadOnboardingExtensions();
            if(_DLLError)
                return false;  

            if (_onboardingSequence.OnboardingSequenceItems.Count == 0)
            {
                Log.Debug("No onboarding sequence items found!!");
                return false;
            }
            foreach (var onboardingItem in _onboardingSequence.OnboardingSequenceItems)
            {
                var type = findOnboardingType(onboardingItem.Id);
                if (type != null)
                {
                    var assembly = Assembly.LoadFrom(type.Assembly.Location);
                    var onboardingExt = (IOnboardingExtension)assembly.CreateInstance(type.FullName);
                    if (onboardingExt != null)
                    {
                        onboardingExt.Initialize(this);
                        _onboardingExtensions.Add(onboardingExt);
                    }
                }
            }

            if (_onboardingExtensions.Count == 0)
            {
                Log.Debug("No onboarding extensions found!!");
                return false;
            }

            return true;
        }

        public bool IsFirstOnboardingExtension(IOnboardingExtension extension)
        {
            int index = _onboardingExtensions.FindIndex(a => a == extension);

            return index == 0;
        }

        public bool IsLastOnboardingExtension(IOnboardingExtension extension)
        {
            int index = _onboardingExtensions.FindIndex(a => a == extension);

            return index == _onboardingExtensions.Count - 1;
        }

        public void OnControlAdded(IOnboardingUserControl c)
        {
            c.OnAdded();
        }

        public bool OnControlPreAdd(IOnboardingUserControl c)
        {
            return c.OnPreAdd();
        }

        public void OnControlRemoved(IOnboardingUserControl c)
        {
            c.OnRemoved();
        }

        public void Quit(IOnboardingExtension source, Reason reason, bool confirm = true)
        {
            EvtQuit?.Invoke(source, reason, confirm);
        }

        public void SetButtonEnable(OnboardingButtonTypes button, bool state)
        {
            if (EvtSetButtonEnabled != null)
            {
                EvtSetButtonEnabled(button, state);
            }
        }

        public void SetButtonText(OnboardingButtonTypes button, String text)
        {
            if (EvtSetButtonText != null)
            {
                EvtSetButtonText(button, text);
            }
        }

        public void SetButtonVisible(OnboardingButtonTypes button, bool visible)
        {
            if (EvtSetButtonVisible != null)
            {
                EvtSetButtonVisible(button, visible);
            }
        }

        private int findOnboardingExtension(IOnboardingExtension extension)
        {
            for (int i = _onboardingExtensions.Count - 1; i >= 0; i--)
            {
                if (_onboardingExtensions[i] == extension)
                {
                    return i;
                }
            }

            return -1;
        }

        private Type findOnboardingType(Guid id)
        {
            foreach (var type in _extensionsTypeCache)
            {
                var guid = getId(type);
                if (guid.Equals(id))
                {
                    return type;
                }
            }

            return null;
        }

        private Guid getId(Type type)
        {
            var descAttribute = DescriptorAttribute.GetDescriptor(type);
            Guid retVal = Guid.Empty;
            if (descAttribute != null)
            {
                retVal = descAttribute.Id;
            }

            return retVal;
        }

        private void loadOnboardingExtensions()
        {
            loadOnboardingExtensionsIntoCache(".");
        }

        private void loadOnboardingExtensionsIntoCache(String dir, bool resursive = true)
        {
            var walker = new DirectoryWalker(dir, "*.dll");
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// <summary>
        /// Callback function for the directory walker. called whenever
        /// it finds a DLL
        /// </summary>
        /// <param name="dllName"></param>
        private void onFileFound(String dllName)
        {
            try
            {

                String extension = Path.GetExtension(dllName);
                if (String.Compare(extension, ".dll", true) == 0 && !_DLLError)
                {
                    var retVal = VerifyDigitalSignature.ValidateCertificate(dllName);
                    if (retVal)
                    {
                        try
                        {
                            VerifyDigitalSignature.Verify(dllName);
                        }
                        catch (Exception ex)
                        {
                            ConfirmBoxSingleOption confirmBoxSingleOption = new ConfirmBoxSingleOption
                            {
                                Prompt = $"The following DLL is not digitally signed \nDLL: {dllName}.\nReason for failure: {ex.Message} \n Status Error: ERO",
                                DecisionPrompt = "ok",
                                LabelFont = 10
                            };
                            confirmBoxSingleOption.BringToFront();
                            confirmBoxSingleOption.TopMost = true;
                            confirmBoxSingleOption.ShowDialog();
                            confirmBoxSingleOption.Dispose();
                            _DLLError = true;
                        }
                    }
                }
                if (!_DLLError)
                {
                    var inputActuatorsAssembly = Assembly.LoadFrom(dllName);
                    foreach (Type type in inputActuatorsAssembly.GetTypes())
                    {
                        if (typeof(IOnboardingExtension).IsAssignableFrom(type))
                        {
                            _extensionsTypeCache.Add(type);
                        }
                    }
                }

            }
            catch
            {
            }
        }

        public class OnboardingHistoryEntry
        {
            public OnboardingHistoryEntry(IOnboardingExtension obe, String step)
            {
                OnboardingExtension = obe;
                StepId = step;
            }

            public IOnboardingExtension OnboardingExtension { get; set; }

            public String StepId { get; set; }
        }
    }
}