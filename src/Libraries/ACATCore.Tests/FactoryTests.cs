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
using System;
using Xunit;

namespace ACATCore.Tests
{
    /// <summary>
    /// Unit tests for the factory pattern implementations:
    /// <see cref="ScannerFactory"/>, <see cref="AgentFactory"/>,
    /// <see cref="ActuatorFactory"/>, and <see cref="WidgetFactory"/>.
    /// </summary>
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

        [Fact]
        public void ScannerFactory_Constructor_NullServiceProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ScannerFactory(null));
        }

        [Fact]
        public void ScannerFactory_Constructor_ValidServiceProvider_Succeeds()
        {
            var sp = BuildServiceProvider();
            var factory = new ScannerFactory(sp);
            Assert.NotNull(factory);
        }

        // ----------------------------------------------------------------
        // ScannerFactory.Create(Type) validation
        // ----------------------------------------------------------------

        [Fact]
        public void ScannerFactory_CreateByType_NullType_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create((Type)null));
        }

        [Fact]
        public void ScannerFactory_CreateByType_NonScannerType_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            var ex = Assert.Throws<ArgumentException>(() => factory.Create(typeof(string)));
            Assert.Contains("IScannerPanel", ex.Message);
        }

        // ----------------------------------------------------------------
        // ScannerFactory.Create(string) validation
        // ----------------------------------------------------------------

        [Fact]
        public void ScannerFactory_CreateByName_NullName_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create((string)null));
        }

        [Fact]
        public void ScannerFactory_CreateByName_EmptyName_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create(string.Empty));
        }

        [Fact]
        public void ScannerFactory_CreateByName_UnknownName_Throws()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.Throws<InvalidOperationException>(() => factory.Create("NonExistentScanner_XYZ_NotReal"));
        }

        // ----------------------------------------------------------------
        // AgentFactory constructor validation
        // ----------------------------------------------------------------

        [Fact]
        public void AgentFactory_Constructor_NullServiceProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AgentFactory(null));
        }

        [Fact]
        public void AgentFactory_Constructor_ValidServiceProvider_Succeeds()
        {
            var sp = BuildServiceProvider();
            var factory = new AgentFactory(sp);
            Assert.NotNull(factory);
        }

        // ----------------------------------------------------------------
        // AgentFactory.Create(Type) validation
        // ----------------------------------------------------------------

        [Fact]
        public void AgentFactory_CreateByType_NullType_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create((Type)null));
        }

        [Fact]
        public void AgentFactory_CreateByType_NonAgentType_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            var ex = Assert.Throws<ArgumentException>(() => factory.Create(typeof(string)));
            Assert.Contains("IApplicationAgent", ex.Message);
        }

        // ----------------------------------------------------------------
        // AgentFactory.Create(string) validation
        // ----------------------------------------------------------------

        [Fact]
        public void AgentFactory_CreateByName_NullName_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create((string)null));
        }

        [Fact]
        public void AgentFactory_CreateByName_EmptyName_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create(string.Empty));
        }

        [Fact]
        public void AgentFactory_CreateByName_UnknownName_Throws()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.Throws<InvalidOperationException>(() => factory.Create("NonExistentAgent_XYZ_NotReal"));
        }

        // ----------------------------------------------------------------
        // ActuatorFactory constructor validation
        // ----------------------------------------------------------------

        [Fact]
        public void ActuatorFactory_Constructor_NullServiceProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ActuatorFactory(null));
        }

        [Fact]
        public void ActuatorFactory_Constructor_ValidServiceProvider_Succeeds()
        {
            var sp = BuildServiceProvider();
            var factory = new ActuatorFactory(sp);
            Assert.NotNull(factory);
        }

        // ----------------------------------------------------------------
        // ActuatorFactory.Create(Type) validation
        // ----------------------------------------------------------------

        [Fact]
        public void ActuatorFactory_CreateByType_NullType_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create((Type)null));
        }

        [Fact]
        public void ActuatorFactory_CreateByType_NonActuatorType_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            var ex = Assert.Throws<ArgumentException>(() => factory.Create(typeof(string)));
            Assert.Contains("IActuator", ex.Message);
        }

        // ----------------------------------------------------------------
        // ActuatorFactory.Create(string) validation
        // ----------------------------------------------------------------

        [Fact]
        public void ActuatorFactory_CreateByName_NullName_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create((string)null));
        }

        [Fact]
        public void ActuatorFactory_CreateByName_EmptyName_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create(string.Empty));
        }

        [Fact]
        public void ActuatorFactory_CreateByName_UnknownName_Throws()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.Throws<InvalidOperationException>(() => factory.Create("NonExistentActuator_XYZ_NotReal"));
        }

        // ----------------------------------------------------------------
        // WidgetFactory constructor validation
        // ----------------------------------------------------------------

        [Fact]
        public void WidgetFactory_Constructor_NullServiceProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new WidgetFactory(null));
        }

        [Fact]
        public void WidgetFactory_Constructor_ValidServiceProvider_Succeeds()
        {
            var sp = BuildServiceProvider();
            var factory = new WidgetFactory(sp);
            Assert.NotNull(factory);
        }

        // ----------------------------------------------------------------
        // WidgetFactory.Create(Type) validation
        // ----------------------------------------------------------------

        [Fact]
        public void WidgetFactory_CreateByType_NullType_Throws()
        {
            var factory = new WidgetFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create((Type)null, new System.Windows.Forms.Control()));
        }

        [Fact]
        public void WidgetFactory_CreateByType_NonWidgetType_Throws()
        {
            var factory = new WidgetFactory(BuildServiceProvider());
            var ex = Assert.Throws<ArgumentException>(() =>
                factory.Create(typeof(string), new System.Windows.Forms.Control()));
            Assert.Contains("Widget", ex.Message);
        }

        [Fact]
        public void WidgetFactory_CreateByType_NullControl_Throws()
        {
            var factory = new WidgetFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() => factory.Create(typeof(Widget), null));
        }

        // ----------------------------------------------------------------
        // WidgetFactory.Create(string) validation
        // ----------------------------------------------------------------

        [Fact]
        public void WidgetFactory_CreateByName_NullName_Throws()
        {
            var factory = new WidgetFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() =>
                factory.Create((string)null, new System.Windows.Forms.Control()));
        }

        [Fact]
        public void WidgetFactory_CreateByName_EmptyName_Throws()
        {
            var factory = new WidgetFactory(BuildServiceProvider());
            Assert.Throws<ArgumentNullException>(() =>
                factory.Create(string.Empty, new System.Windows.Forms.Control()));
        }

        [Fact]
        public void WidgetFactory_CreateByName_UnknownName_Throws()
        {
            var factory = new WidgetFactory(BuildServiceProvider());
            Assert.Throws<InvalidOperationException>(() =>
                factory.Create("NonExistentWidget_XYZ_NotReal", new System.Windows.Forms.Control()));
        }

        // ----------------------------------------------------------------
        // DI container registration
        // ----------------------------------------------------------------

        [Fact]
        public void DI_IScannerFactory_IsRegistered_AfterAddPanelManagement()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddPanelManagement();
            var sp = services.BuildServiceProvider();

            var factory = sp.GetService<IScannerFactory>();
            Assert.NotNull(factory);
            Assert.IsType<ScannerFactory>(factory);
        }

        [Fact]
        public void DI_IAgentFactory_IsRegistered_AfterAddAgentManagement()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAgentManagement();
            var sp = services.BuildServiceProvider();

            var factory = sp.GetService<IAgentFactory>();
            Assert.NotNull(factory);
            Assert.IsType<AgentFactory>(factory);
        }

        [Fact]
        public void DI_IActuatorFactory_IsRegistered_AfterAddActuatorManagement()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddActuatorManagement();
            var sp = services.BuildServiceProvider();

            var factory = sp.GetService<IActuatorFactory>();
            Assert.NotNull(factory);
            Assert.IsType<ActuatorFactory>(factory);
        }

        [Fact]
        public void DI_IWidgetFactory_IsRegistered_AfterAddWidgetManagement()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddWidgetManagement();
            var sp = services.BuildServiceProvider();

            var factory = sp.GetService<IWidgetFactory>();
            Assert.NotNull(factory);
            Assert.IsType<WidgetFactory>(factory);
        }

        [Fact]
        public void DI_AllFactories_AreRegistered_AfterAddACATCoreModules()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATCoreModules();
            var sp = services.BuildServiceProvider();

            Assert.NotNull(sp.GetService<IScannerFactory>());
            Assert.NotNull(sp.GetService<IAgentFactory>());
            Assert.NotNull(sp.GetService<IActuatorFactory>());
            Assert.NotNull(sp.GetService<IWidgetFactory>());
        }

        [Fact]
        public void DI_Factories_AreSingletons()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddPanelManagement();
            services.AddAgentManagement();
            services.AddActuatorManagement();
            services.AddWidgetManagement();
            var sp = services.BuildServiceProvider();

            Assert.Same(sp.GetService<IScannerFactory>(), sp.GetService<IScannerFactory>());
            Assert.Same(sp.GetService<IAgentFactory>(), sp.GetService<IAgentFactory>());
            Assert.Same(sp.GetService<IActuatorFactory>(), sp.GetService<IActuatorFactory>());
            Assert.Same(sp.GetService<IWidgetFactory>(), sp.GetService<IWidgetFactory>());
        }

        // ----------------------------------------------------------------
        // IScannerFactory interface compliance
        // ----------------------------------------------------------------

        [Fact]
        public void ScannerFactory_ImplementsIScannerFactory()
        {
            var factory = new ScannerFactory(BuildServiceProvider());
            Assert.IsAssignableFrom<IScannerFactory>(factory);
        }

        [Fact]
        public void AgentFactory_ImplementsIAgentFactory()
        {
            var factory = new AgentFactory(BuildServiceProvider());
            Assert.IsAssignableFrom<IAgentFactory>(factory);
        }

        [Fact]
        public void ActuatorFactory_ImplementsIActuatorFactory()
        {
            var factory = new ActuatorFactory(BuildServiceProvider());
            Assert.IsAssignableFrom<IActuatorFactory>(factory);
        }

        [Fact]
        public void WidgetFactory_ImplementsIWidgetFactory()
        {
            var factory = new WidgetFactory(BuildServiceProvider());
            Assert.IsAssignableFrom<IWidgetFactory>(factory);
        }
    }
}
