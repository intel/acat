////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace ACAT.Core.WidgetManagement
{
    /// <summary>
    /// Factory interface for creating <see cref="Widget"/> instances.
    /// Supports both type-based and name-based instantiation with
    /// dependency injection for services such as loggers.
    /// </summary>
    public interface IWidgetFactory
    {
        /// <summary>
        /// Creates a widget instance of the specified concrete type.
        /// </summary>
        /// <param name="widgetType">
        /// The concrete type to instantiate. Must derive from <see cref="Widget"/>.
        /// </param>
        /// <param name="control">The WinForms control to associate with the widget.</param>
        /// <param name="parent">Optional parent widget.</param>
        /// <returns>A new widget instance.</returns>
        Widget Create(Type widgetType, Control control, Widget parent = null);

        /// <summary>
        /// Creates a widget instance identified by its simple class name.
        /// The name is matched against types that derive from <see cref="Widget"/>
        /// in the currently loaded assemblies.
        /// </summary>
        /// <param name="widgetTypeName">
        /// The simple class name of the widget (e.g. "ScannerButtonControl").
        /// </param>
        /// <param name="control">The WinForms control to associate with the widget.</param>
        /// <param name="parent">Optional parent widget.</param>
        /// <returns>A new widget instance.</returns>
        Widget Create(string widgetTypeName, Control control, Widget parent = null);
    }

    /// <summary>
    /// Default implementation of <see cref="IWidgetFactory"/>.
    /// Uses <see cref="ActivatorUtilities"/> so that constructor-injected
    /// services (e.g. <see cref="ILogger{T}"/>) are resolved from the DI container.
    /// The <c>control</c> argument passed to <see cref="Create(Type,Control,Widget)"/> is
    /// forwarded directly to the widget constructor.
    /// </summary>
    public class WidgetFactory : IWidgetFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WidgetFactory> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="WidgetFactory"/>.
        /// </summary>
        /// <param name="serviceProvider">The DI service provider.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public WidgetFactory(IServiceProvider serviceProvider, ILogger<WidgetFactory> logger = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger;
        }

        /// <inheritdoc/>
        public Widget Create(Type widgetType, Control control, Widget parent = null)
        {
            if (widgetType == null)
                throw new ArgumentNullException(nameof(widgetType));

            if (!typeof(Widget).IsAssignableFrom(widgetType))
                throw new ArgumentException(
                    $"Type '{widgetType.FullName}' does not derive from Widget.",
                    nameof(widgetType));

            if (control == null)
                throw new ArgumentNullException(nameof(control));

            try
            {
                _logger?.LogDebug("Creating widget of type {TypeName}", widgetType.FullName);
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, widgetType, control);
                var widget = (Widget)instance;
                if (parent != null)
                    widget.Parent = parent;
                return widget;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create widget of type {TypeName}", widgetType.FullName);
                throw;
            }
        }

        /// <inheritdoc/>
        public Widget Create(string widgetTypeName, Control control, Widget parent = null)
        {
            if (string.IsNullOrWhiteSpace(widgetTypeName))
                throw new ArgumentNullException(nameof(widgetTypeName));

            Type widgetType = WidgetManager.GetWidgetType(widgetTypeName);
            if (widgetType == null)
                throw new InvalidOperationException(
                    $"No widget type found for name '{widgetTypeName}'.");

            return Create(widgetType, control, parent);
        }
    }
}
