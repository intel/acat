////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// FactoryTests.cs
//
// Unit tests for IScannerFactory, IAgentFactory, IActuatorFactory, and
// IWidgetFactory implementations.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.DependencyInjection;
using ACAT.Core.PanelManagement;
using ACAT.Core.WidgetManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ACATCore.Tests.Architecture
{
    /// <summary>
    /// Unit tests for the factory pattern implementations:
    /// <see cref="ScannerFactory"/>, <see cref="AgentFactory"/>,
    /// <see cref="ActuatorFactory"/>, and <see cref="WidgetFactory"/>.
    /// </summary>
    [TestClass]
    public class FactoryTests
    {
        private static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            return services.BuildServiceProvider();
        }

        // ----------------------------------------------------------------
        // ScannerFactory constructor validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void ScannerFactory_Constructor_NullServiceProvider_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ScannerFactory(null));
        }

        [TestMethod]
        public void ScannerFactory_Constructor_ValidServiceProvider_Succeeds()
        {
            var sp = BuildServiceProvider();
            var factory = new ScannerFactory(sp);
            Assert.IsNotNull(factory);
        }

        // ----------------------------------------------------------------
        // ScannerFactory.Create(Type) validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void ScannerFactory_CreateByType_NullType_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.ThrowsException<ArgumentNullException>(() => factory.Create((Type)null));
        }

        [TestMethod]
        public void ScannerFactory_CreateByType_NonScannerType_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            var ex = Assert.ThrowsException<ArgumentException>(() => factory.Create(typeof(string)));
            StringAssert.Contains(ex.Message, "IScannerPanel");
        }

        // ----------------------------------------------------------------
        // ScannerFactory.Create(string) validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void ScannerFactory_CreateByName_NullName_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.ThrowsException<ArgumentNullException>(() => factory.Create((string)null));
        }

        [TestMethod]
        public void ScannerFactory_CreateByName_EmptyName_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.ThrowsException<ArgumentNullException>(() => factory.Create(string.Empty));
        }

        [TestMethod]
        public void ScannerFactory_CreateByName_UnknownName_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.ThrowsException<InvalidOperationException>(() =>
                factory.Create("NonExistentScanner_XYZ_NotReal"));
        }

        // ----------------------------------------------------------------
        // AgentFactory constructor validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void AgentFactory_Constructor_NullServiceProvider_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new AgentFactory(null));
        }

        [TestMethod]
        public void AgentFactory_Constructor_ValidServiceProvider_Succeeds()
        {
            var sp = BuildServiceProvider();
            var factory = new AgentFactory(sp);
            Assert.IsNotNull(factory);
        }

        // ----------------------------------------------------------------
        // AgentFactory.Create(Type) validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void AgentFactory_CreateByType_NullType_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.ThrowsException<ArgumentNullException>(() => factory.Create((Type)null));
        }

        [TestMethod]
        public void AgentFactory_CreateByType_NonAgentType_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            var ex = Assert.ThrowsException<ArgumentException>(() => factory.Create(typeof(string)));
            StringAssert.Contains(ex.Message, "IApplicationAgent");
        }

        // ----------------------------------------------------------------
        // AgentFactory.Create(string) validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void AgentFactory_CreateByName_NullName_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.ThrowsException<ArgumentNullException>(() => factory.Create((string)null));
        }

        [TestMethod]
        public void AgentFactory_CreateByName_EmptyName_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.ThrowsException<ArgumentNullException>(() => factory.Create(string.Empty));
        }

        [TestMethod]
        public void AgentFactory_CreateByName_UnknownName_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.ThrowsException<InvalidOperationException>(() =>
                factory.Create("NonExistentAgent_XYZ_NotReal"));
        }

        // ----------------------------------------------------------------
        // ActuatorFactory constructor validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void ActuatorFactory_Constructor_NullServiceProvider_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ActuatorFactory(null));
        }

        [TestMethod]
        public void ActuatorFactory_Constructor_ValidServiceProvider_Succeeds()
        {
            var sp = BuildServiceProvider();
            var factory = new ActuatorFactory(sp);
            Assert.IsNotNull(factory);
        }

        // ----------------------------------------------------------------
        // ActuatorFactory.Create(Type) validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void ActuatorFactory_CreateByType_NullType_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.ThrowsException<ArgumentNullException>(() => factory.Create((Type)null));
        }

        [TestMethod]
        public void ActuatorFactory_CreateByType_NonActuatorType_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            var ex = Assert.ThrowsException<ArgumentException>(() => factory.Create(typeof(string)));
            StringAssert.Contains(ex.Message, "IActuator");
        }

        // ----------------------------------------------------------------
        // ActuatorFactory.Create(string) validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void ActuatorFactory_CreateByName_NullName_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.ThrowsException<ArgumentNullException>(() => factory.Create((string)null));
        }

        [TestMethod]
        public void ActuatorFactory_CreateByName_EmptyName_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.ThrowsException<ArgumentNullException>(() => factory.Create(string.Empty));
        }

        [TestMethod]
        public void ActuatorFactory_CreateByName_UnknownName_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.ThrowsException<InvalidOperationException>(() =>
                factory.Create("NonExistentActuator_XYZ_NotReal"));
        }

        // ----------------------------------------------------------------
        // WidgetFactory constructor validation
        // ----------------------------------------------------------------

        [TestMethod]
        public void WidgetFactory_Constructor_NullServiceProvider_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new WidgetFactory(null));
        }

        [TestMethod]
        public void WidgetFactory_Constructor_ValidServiceProvider_Succeeds()
        {
            var sp = BuildServiceProvider();
            var factory = new WidgetFactory(sp);
            Assert.IsNotNull(factory);
        }

        // ----------------------------------------------------------------
        // DI container registration
        // ----------------------------------------------------------------

        [TestMethod]
        public void DI_IScannerFactory_IsRegistered_AfterAddPanelManagement()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddPanelManagement();
            var sp = services.BuildServiceProvider();

            var factory = sp.GetService<IScannerFactory>();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(ScannerFactory));
        }

        [TestMethod]
        public void DI_IAgentFactory_IsRegistered_AfterAddAgentManagement()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAgentManagement();
            var sp = services.BuildServiceProvider();

            var factory = sp.GetService<IAgentFactory>();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(AgentFactory));
        }

        [TestMethod]
        public void DI_IActuatorFactory_IsRegistered_AfterAddActuatorManagement()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddActuatorManagement();
            var sp = services.BuildServiceProvider();

            var factory = sp.GetService<IActuatorFactory>();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(ActuatorFactory));
        }

        [TestMethod]
        public void DI_IWidgetFactory_IsRegistered_AfterAddWidgetManagement()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddWidgetManagement();
            var sp = services.BuildServiceProvider();

            var factory = sp.GetService<IWidgetFactory>();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(WidgetFactory));
        }

        [TestMethod]
        public void DI_Factories_AreSingletons()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddPanelManagement();
            services.AddAgentManagement();
            services.AddActuatorManagement();
            services.AddWidgetManagement();
            var sp = services.BuildServiceProvider();

            Assert.AreSame(sp.GetService<IScannerFactory>(), sp.GetService<IScannerFactory>());
            Assert.AreSame(sp.GetService<IAgentFactory>(), sp.GetService<IAgentFactory>());
            Assert.AreSame(sp.GetService<IActuatorFactory>(), sp.GetService<IActuatorFactory>());
            Assert.AreSame(sp.GetService<IWidgetFactory>(), sp.GetService<IWidgetFactory>());
        }

        // ----------------------------------------------------------------
        // Interface compliance
        // ----------------------------------------------------------------

        [TestMethod]
        public void ScannerFactory_ImplementsIScannerFactory()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.IsInstanceOfType(factory, typeof(IScannerFactory));
        }

        [TestMethod]
        public void AgentFactory_ImplementsIAgentFactory()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.IsInstanceOfType(factory, typeof(IAgentFactory));
        }

        [TestMethod]
        public void ActuatorFactory_ImplementsIActuatorFactory()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.IsInstanceOfType(factory, typeof(IActuatorFactory));
        }

        [TestMethod]
        public void WidgetFactory_ImplementsIWidgetFactory()
        {
            var factory = new WidgetFactory(BuildServiceProvider());
            Assert.IsInstanceOfType(factory, typeof(IWidgetFactory));
        }
    }
}
