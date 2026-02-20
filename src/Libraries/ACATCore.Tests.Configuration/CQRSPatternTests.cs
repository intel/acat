////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CQRSPatternTests.cs
//
// Tests for the CQRS interfaces and sample implementations introduced in
// ACAT.Core.Patterns.CQRS.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.Patterns.CQRS;
using ACAT.Core.Patterns.CQRS.Samples;
using ACAT.Core.PanelManagement.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ACATCore.Tests.Configuration
{
    /// <summary>
    /// Tests for CQRS interfaces and sample command/query implementations.
    /// </summary>
    [TestClass]
    public class CQRSPatternTests
    {
        // ----------------------------------------------------------------
        // Interface contract – CreatePanelCommand
        // ----------------------------------------------------------------

        [TestMethod]
        public void CreatePanelCommand_ImplementsICommand()
        {
            var command = new CreatePanelCommand("TestPanel");

            Assert.IsInstanceOfType(command, typeof(ICommand));
        }

        [TestMethod]
        public void CreatePanelCommand_StoresPanelClass()
        {
            var command = new CreatePanelCommand("AlphabetScanner");

            Assert.AreEqual("AlphabetScanner", command.PanelClass);
        }

        [TestMethod]
        public void CreatePanelCommand_DefaultTitleIsNull()
        {
            var command = new CreatePanelCommand("AlphabetScanner");

            Assert.IsNull(command.Title);
        }

        [TestMethod]
        public void CreatePanelCommand_DefaultStartupArgIsNull()
        {
            var command = new CreatePanelCommand("AlphabetScanner");

            Assert.IsNull(command.StartupArg);
        }

        [TestMethod]
        public void CreatePanelCommand_StoresTitleAndStartupArg()
        {
            var arg = new StartupArg("TestScreen");
            var command = new CreatePanelCommand("AlphabetScanner", "My Title", arg);

            Assert.AreEqual("AlphabetScanner", command.PanelClass);
            Assert.AreEqual("My Title", command.Title);
            Assert.AreSame(arg, command.StartupArg);
        }

        // ----------------------------------------------------------------
        // Interface contract – HandleActuatorSwitchCommand
        // ----------------------------------------------------------------

        [TestMethod]
        public void HandleActuatorSwitchCommand_ImplementsICommand()
        {
            var command = new HandleActuatorSwitchCommand(ActuatorSwitchAction.Pause);

            Assert.IsInstanceOfType(command, typeof(ICommand));
        }

        [TestMethod]
        public void HandleActuatorSwitchCommand_StoresPauseAction()
        {
            var command = new HandleActuatorSwitchCommand(ActuatorSwitchAction.Pause);

            Assert.AreEqual(ActuatorSwitchAction.Pause, command.Action);
        }

        [TestMethod]
        public void HandleActuatorSwitchCommand_StoresResumeAction()
        {
            var command = new HandleActuatorSwitchCommand(ActuatorSwitchAction.Resume);

            Assert.AreEqual(ActuatorSwitchAction.Resume, command.Action);
        }

        // ----------------------------------------------------------------
        // Interface contract – query types
        // ----------------------------------------------------------------

        [TestMethod]
        public void GetConfigurationValueQuery_ImplementsIQuery()
        {
            var query = new GetConfigurationValueQuery("SomeKey");

            Assert.IsInstanceOfType(query, typeof(IQuery<string>));
        }

        [TestMethod]
        public void GetConfigurationValueQuery_StoresKey()
        {
            var query = new GetConfigurationValueQuery("MyKey");

            Assert.AreEqual("MyKey", query.Key);
        }

        [TestMethod]
        public void GetActiveAgentNameQuery_ImplementsIQuery()
        {
            var query = new GetActiveAgentNameQuery();

            Assert.IsInstanceOfType(query, typeof(IQuery<string>));
        }

        // ----------------------------------------------------------------
        // GetConfigurationValueQueryHandler – behaviour tests
        // ----------------------------------------------------------------

        [TestMethod]
        public void GetConfigurationValueQueryHandler_ReturnsStoredOverrideValue()
        {
            var config = new EnvironmentConfiguration();
            config.SetOverride("Theme", "Dark");
            var handler = new GetConfigurationValueQueryHandler(config);

            string result = handler.Handle(new GetConfigurationValueQuery("Theme"));

            Assert.AreEqual("Dark", result);
        }

        [TestMethod]
        public void GetConfigurationValueQueryHandler_ReturnsNullForMissingKey()
        {
            var config = new EnvironmentConfiguration();
            var handler = new GetConfigurationValueQueryHandler(config);

            string result = handler.Handle(new GetConfigurationValueQuery("NonExistentKey"));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetConfigurationValueQueryHandler_ReturnsUpdatedValue_AfterOverrideChanges()
        {
            var config = new EnvironmentConfiguration();
            config.SetOverride("Skin", "Light");
            var handler = new GetConfigurationValueQueryHandler(config);

            config.SetOverride("Skin", "Dark");
            string result = handler.Handle(new GetConfigurationValueQuery("Skin"));

            Assert.AreEqual("Dark", result);
        }

        // ----------------------------------------------------------------
        // HandleActuatorSwitchCommandHandler – behaviour tests via fake
        // ----------------------------------------------------------------

        [TestMethod]
        public void HandleActuatorSwitchCommandHandler_Pause_CallsPauseOnManager()
        {
            var fake = new FakeActuatorManager();
            var handler = new HandleActuatorSwitchCommandHandler(fake);

            handler.Handle(new HandleActuatorSwitchCommand(ActuatorSwitchAction.Pause));

            Assert.IsTrue(fake.PauseCalled);
            Assert.IsFalse(fake.ResumeCalled);
        }

        [TestMethod]
        public void HandleActuatorSwitchCommandHandler_Resume_CallsResumeOnManager()
        {
            var fake = new FakeActuatorManager();
            var handler = new HandleActuatorSwitchCommandHandler(fake);

            handler.Handle(new HandleActuatorSwitchCommand(ActuatorSwitchAction.Resume));

            Assert.IsTrue(fake.ResumeCalled);
            Assert.IsFalse(fake.PauseCalled);
        }

        // ----------------------------------------------------------------
        // GetActiveAgentNameQueryHandler – behaviour tests via fake
        // ----------------------------------------------------------------

        [TestMethod]
        public void GetActiveAgentNameQueryHandler_ReturnsAgentName()
        {
            var fake = new FakeAgentManager { CurrentAgentName = "NotepadAgent" };
            var handler = new GetActiveAgentNameQueryHandler(fake);

            string result = handler.Handle(new GetActiveAgentNameQuery());

            Assert.AreEqual("NotepadAgent", result);
        }

        [TestMethod]
        public void GetActiveAgentNameQueryHandler_ReturnsEmptyStringWhenNoAgent()
        {
            var fake = new FakeAgentManager { CurrentAgentName = null };
            var handler = new GetActiveAgentNameQueryHandler(fake);

            string result = handler.Handle(new GetActiveAgentNameQuery());

            Assert.AreEqual(string.Empty, result);
        }
    }
}
