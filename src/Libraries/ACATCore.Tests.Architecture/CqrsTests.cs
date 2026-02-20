////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CqrsTests.cs
//
// Unit tests for the CQRS pattern interfaces and sample implementations.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Patterns.CQRS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ACATCore.Tests.Architecture
{
    /// <summary>
    /// Unit tests for the CQRS pattern interfaces and sample command/query objects.
    /// </summary>
    [TestClass]
    public class CqrsTests
    {
        // -----------------------------------------------------------------------
        // Command objects
        // -----------------------------------------------------------------------

        [TestMethod]
        public void ShowPanelCommand_IsICommand()
        {
            var cmd = new ShowPanelCommand("TestPanel");
            Assert.IsInstanceOfType(cmd, typeof(ICommand));
            Assert.AreEqual("TestPanel", cmd.PanelName);
        }

        [TestMethod]
        public void HidePanelCommand_IsICommand()
        {
            var cmd = new HidePanelCommand("TestPanel");
            Assert.IsInstanceOfType(cmd, typeof(ICommand));
            Assert.AreEqual("TestPanel", cmd.PanelName);
        }

        [TestMethod]
        public void HandleActuatorSwitchCommand_IsICommand()
        {
            var cmd = new HandleActuatorSwitchCommand("LeftSwitch", "data");
            Assert.IsInstanceOfType(cmd, typeof(ICommand));
            Assert.AreEqual("LeftSwitch", cmd.SwitchName);
            Assert.AreEqual("data", cmd.SwitchData);
        }

        [TestMethod]
        public void HandleActuatorSwitchCommand_NullData_DefaultsToNull()
        {
            var cmd = new HandleActuatorSwitchCommand("Switch");
            Assert.IsNull(cmd.SwitchData);
        }

        // -----------------------------------------------------------------------
        // Query objects
        // -----------------------------------------------------------------------

        [TestMethod]
        public void GetActivePanelQuery_IsIQuery()
        {
            var q = new GetActivePanelQuery();
            Assert.IsInstanceOfType(q, typeof(IQuery<string>));
        }

        [TestMethod]
        public void GetAllPanelNamesQuery_IsIQuery()
        {
            var q = new GetAllPanelNamesQuery();
            Assert.IsInstanceOfType(q, typeof(IQuery<IReadOnlyList<string>>));
        }

        [TestMethod]
        public void GetConfigurationValueQuery_Properties_SetCorrectly()
        {
            var q = new GetConfigurationValueQuery("theme", "default");
            Assert.IsInstanceOfType(q, typeof(IQuery<string>));
            Assert.AreEqual("theme", q.Key);
            Assert.AreEqual("default", q.DefaultValue);
        }

        [TestMethod]
        public void GetConfigurationValueQuery_DefaultValueIsNull_WhenNotProvided()
        {
            var q = new GetConfigurationValueQuery("key");
            Assert.IsNull(q.DefaultValue);
        }

        // -----------------------------------------------------------------------
        // Concrete handler implementations
        // -----------------------------------------------------------------------

        [TestMethod]
        public void CommandHandler_Handle_ExecutesAction()
        {
            var handler = new StubShowPanelHandler();
            handler.Handle(new ShowPanelCommand("Alpha"));
            Assert.AreEqual("Alpha", handler.LastShownPanel);
        }

        [TestMethod]
        public void QueryHandler_Handle_ReturnsExpectedResult()
        {
            var handler = new StubActivePanelQueryHandler("BetaPanel");
            var result = handler.Handle(new GetActivePanelQuery());
            Assert.AreEqual("BetaPanel", result);
        }

        // -----------------------------------------------------------------------
        // Stub implementations (local to this test class)
        // -----------------------------------------------------------------------

        private sealed class StubShowPanelHandler : ICommandHandler<ShowPanelCommand>
        {
            public string LastShownPanel { get; private set; }
            public void Handle(ShowPanelCommand command) => LastShownPanel = command.PanelName;
        }

        private sealed class StubActivePanelQueryHandler : IQueryHandler<GetActivePanelQuery, string>
        {
            private readonly string _panel;
            public StubActivePanelQueryHandler(string panel) { _panel = panel; }
            public string Handle(GetActivePanelQuery query) => _panel;
        }
    }
}
