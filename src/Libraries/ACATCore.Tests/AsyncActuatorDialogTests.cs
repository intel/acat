////////////////////////////////////////////////////////////////////////////
//
// Copyright 2026 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.ActuatorManagement.Settings;
using Moq;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;

namespace ACATCore.Tests
{
    public class AsyncActuatorDialogTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SavePreparation_WhenReloadedActuatorIsMissing_RejectsSave(bool keyboard)
        {
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var thread = new Thread(() =>
            {
                try
                {
                    var actuator = new Mock<IActuator>();
                    actuator.SetupGet(value => value.Name).Returns("Missing actuator");
                    using (Form form = keyboard
                        ? (Form)new ConfigureKeyboardActuatorForm { Actuator = actuator.Object }
                        : new ConfigureSwitchesForm { Actuator = actuator.Object })
                    {
                        var prepareSave = form.GetType().GetMethod("updateDataFromUI",
                            BindingFlags.Instance | BindingFlags.NonPublic);
                        Assert.NotNull(prepareSave);
                        bool prepared = (bool)prepareSave.Invoke(form, new object[] { new ActuatorConfig() });
                        Assert.False(prepared);
                        Assert.NotEqual(DialogResult.OK, form.DialogResult);
                    }
                    completion.SetResult(true);
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            await completion.Task;
        }
    }
}
