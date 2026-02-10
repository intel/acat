////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.CoreInterfaces;
using ACAT.Core.Utility;
using ACAT.Core.Utility.TypeLoader;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Extensions.Onboarding.UI
{
    /// <summary>
    /// Represents the onboarding wizard controller. Controls navigation
    /// between the extensions and between the steps in each extension
    /// </summary>
    public class OnboardingWizard : IOnboardingWizard
    {
        private static volatile bool _DLLError = false;
        private readonly ILogger<OnboardingWizard> _logger;
        private int _extensionIndex = -1;
        private readonly List<Type> _extensionsTypeCache = new();
        private readonly List<OnboardingHistoryEntry> _history = new();
        private readonly List<IOnboardingExtension> _onboardingExtensions = new();
        private OnboardingSequence _onboardingSequence;

        public OnboardingWizard()
        {
            _logger = LoggingConfiguration.CreateLogger<OnboardingWizard>();
        }
        private readonly TypeLoader<IOnboardingExtension> _TypeLoader = new();


        public delegate void AddCustomButtonDelegate(Control control, OnboardingButtonTypes buttonType);

        public delegate void GoBackDelegate(IOnboardingExtension source);

        public delegate void GotoNextDelegate(IOnboardingExtension source);

        public delegate void QuitDelegate(IOnboardingExtension source, Reason reason, bool confirm);

        public delegate void SetButtonEnabledDelegate(OnboardingButtonTypes button, bool enable);

        public delegate void SetButtonTextDelegate(OnboardingButtonTypes button, string text);

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
            EvtAddCustomButton?.Invoke(control, buttonType);
        }

        public void AddToHistory(IOnboardingExtension obe, string step)
        {
            _history.Add(new OnboardingHistoryEntry(obe, step));
        }

        public IOnboardingExtension GetNextOnboardingExtension()
        {
            if (_extensionIndex + 1 >= _onboardingExtensions.Count)
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
            if (_DLLError)
                return false;

            if (_onboardingSequence.OnboardingSequenceItems.Count == 0)
            {
                _logger.LogDebug("No onboarding sequence items found!!");
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
                _logger.LogDebug("No onboarding extensions found!!");
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
            EvtSetButtonEnabled?.Invoke(button, state);
        }

        public void SetButtonText(OnboardingButtonTypes button, string text)
        {
            EvtSetButtonText?.Invoke(button, text);
        }

        public void SetButtonVisible(OnboardingButtonTypes button, bool visible)
        {
            EvtSetButtonVisible?.Invoke(button, visible);
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
            var descAttribute = ClassDescriptorAttribute.GetDescriptor(type);
            Guid retVal = Guid.Empty;
            if (descAttribute != null)
            {
                retVal = descAttribute.Id;
            }

            return retVal;
        }

        private void loadOnboardingExtensions()
        {
            // Load all onboarding components from myself first
            // Get the current assembly
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            // Find all types that implement IOnboardingExtension
            var onboardingTypes = currentAssembly
                .GetTypes()
                .Where(t => typeof(IOnboardingExtension).IsAssignableFrom(t)
                            && t.IsClass
                            && !t.IsAbstract)
                .ToList();

            _extensionsTypeCache.AddRange(onboardingTypes);
        }

        public class OnboardingHistoryEntry
        {
            public OnboardingHistoryEntry(IOnboardingExtension obe, string step)
            {
                OnboardingExtension = obe;
                StepId = step;
            }

            public IOnboardingExtension OnboardingExtension { get; set; }

            public string StepId { get; set; }
        }
    }
}