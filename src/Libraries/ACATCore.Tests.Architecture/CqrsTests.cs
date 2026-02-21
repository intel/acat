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
using ACAT.Core.Patterns.CQRS.Samples;
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
        // Panel command objects (defined in ACAT.Core.Patterns.CQRS)
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

        // -----------------------------------------------------------------------
        // Panel query objects (defined in ACAT.Core.Patterns.CQRS)
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

        // -----------------------------------------------------------------------
        // Sample command/query objects (defined in ACAT.Core.Patterns.CQRS.Samples)
        // -----------------------------------------------------------------------

        [TestMethod]
        public void HandleActuatorSwitchCommand_IsICommand()
        {
            var cmd = new HandleActuatorSwitchCommand(ActuatorSwitchAction.Pause);
            Assert.IsInstanceOfType(cmd, typeof(ICommand));
            Assert.AreEqual(ActuatorSwitchAction.Pause, cmd.Action);
        }

        [TestMethod]
        public void HandleActuatorSwitchCommand_Resume_StoresAction()
        {
            var cmd = new HandleActuatorSwitchCommand(ActuatorSwitchAction.Resume);
            Assert.AreEqual(ActuatorSwitchAction.Resume, cmd.Action);
        }

        [TestMethod]
        public void GetConfigurationValueQuery_IsIQuery()
        {
            var q = new GetConfigurationValueQuery("theme");
            Assert.IsInstanceOfType(q, typeof(IQuery<string>));
            Assert.AreEqual("theme", q.Key);
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
