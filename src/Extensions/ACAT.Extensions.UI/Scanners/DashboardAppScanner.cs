 using ACAT.Core.AgentManagement;
using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.Patterns.CQRS;
using ACAT.Core.Patterns.CQRS.Samples;
using ACAT.Core.UserControlManagement;
using ACAT.Core.UserControlManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement.Interfaces;
using ACAT.Extension;
using ACAT.Extension.AppAgents.ChromeBrowser;
using ACAT.Extension.CommandHandlers;
using ACAT.Extension.UI;
using ACAT.Extension.UI.ScannerForms;
using ACAT.Extensions.UI.UserControls.Toolbars;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.Scanners
{
    [ClassDescriptor("D3F4A1B2-3C4D-5E6F-7A8B-9A0B1C2D3E4F",
                Name = "DashboardAppScanner",
                Description = "Scanner for Dashboard Applications")]
    [DesignerCategory("code")]
    public partial class DashboardAppScanner : GenericScannerForm
    {
        private readonly ILogger<DashboardAppScanner> _logger;
        private TableLayoutPanel panelDashboardControls;
        private TableLayoutPanel panelTopToolbar;
        private TableLayoutPanel ScannerBorder;

        private UserControl currentPanel = null;

        public DashboardAppScanner() : base()
        {
            _logger = LoggingConfiguration.CreateLogger<DashboardAppScanner>();
            _dispatcher = new DashboardAppDispatcher(this);
            _launchAppAgent = Context.AppAgentMgr.GetFunctionalAgentByName("LaunchAppAgent") as IFunctionalAgent;
            if (_launchAppAgent == null)
            {
                _logger.LogError("LaunchAppAgent not found. Ensure it is registered in the ACAT configuration.");
            }
        }

        private IFunctionalAgent _launchAppAgent { get; set; }

        public override DefaultCommandDispatcher _dispatcher { get; }
        public override RunCommandDispatcher CommandDispatcher => _dispatcher;
        public ScannerStatusBar ScannerStatusBar
        {
            get { return ScannerCommon.StatusBar; }
        }


        public override ITextController TextController => ScannerCommon.TextController;
        
        public override bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            switch (arg.Command)
            {
                case "CmdTalkSentenceMode":
                case "CmdTalkPhraseMode":
                case "CmdTalkShorthandMode":
                case "CmdSwitchWindow":
                case "Exit":
                    arg.Handled = true;
                    arg.Enabled = true;
                    break;
                default:
                    _scannerHelper.CheckCommandEnabled(arg);
                    break;
            }

            return true;
        }

        public override bool HandleInitialize(StartupArg startup)
        {
            ScannerCommon.UserControlManager.GridScanIterations = Common.AppPreferences.GridScanIterations;
            bool success = ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelTopToolbar, "toolbar", "ToolbarUserControl");
            success = success && ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelDashboardControls, "dashboard", "ToolsMenuUserControl");

            return success;
        }

        //private bool _transparencyIncreasing = false;

        protected override void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(192F, 192F);
            this.AutoScaleMode = AutoScaleMode.Dpi;

            this.ScannerBorder = new TableLayoutPanel();
            this.panelTopToolbar = new TableLayoutPanel();
            this.panelDashboardControls = new TableLayoutPanel();

            this.panelTopToolbar.SuspendLayout();
            this.panelDashboardControls.SuspendLayout();
            this.SuspendLayout();

            InitializeTopToolbar();
            InitializeDashboard();

            this.ScannerBorder.BackColor = Color.Transparent;
            this.ScannerBorder.Name = "DashboardScannerBorder";
            this.ScannerBorder.AccessibleName = "DashboardScannerBorder";
            this.ScannerBorder.ColumnCount = 1;
            this.ScannerBorder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.ScannerBorder.RowCount = 2;
            this.ScannerBorder.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.ScannerBorder.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.ScannerBorder.Dock = DockStyle.Fill;
            this.ScannerBorder.AutoSize = true;
            this.ScannerBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.ScannerBorder.Controls.Add(this.panelTopToolbar, 0, 0);
            this.ScannerBorder.Controls.Add(this.panelDashboardControls, 0, 1);

            this.Text = "ACAT";
            this.Name = "DashboardAppScanner";
            this.BackColor = Color.FromArgb(35, 36, 51);
            this.ForeColor = Color.White;
            this.Controls.Add(ScannerBorder);
            this.ShowInTaskbar = true;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.StartPosition = FormStartPosition.WindowsDefaultLocation;

            this.panelTopToolbar.ResumeLayout(true);
            this.panelDashboardControls.ResumeLayout(true);
            this.ScannerBorder.ResumeLayout(true);
            this.ResumeLayout(true);

            // Subscribe to the change event on the panelDashboardControls to
            // handle setting what the current panel is.
            this.panelDashboardControls.ControlAdded += (s, e) =>
            {
                if (e.Control is UserControl userControl)
                {
                    currentPanel = userControl;
                    _logger.LogDebug("Current panel set to: {PanelName}", currentPanel.GetType().Name);
                }
            };
        }

        protected override void ScannerFormLoaded(object sender, EventArgs e)
        {
            Icon icon = ImageUtils.GetEntryAssemblyIcon();
            if (icon != null)
            {
                Icon = icon;
            }

            panelTopToolbar.Focus();
            ScannerCommon.OnLoad();

            SetColorScheme();

        }


        protected override void SubscribeToEvents()
        {
            Load += ScannerFormLoaded;
            //Shown += ScannerShown;
            FormClosing += ScannerFormClosing;
        }

        public bool HandleCmdPointerControl()
        {
            bool success = ScannerCommon.UserControlManager.PushUserControlByKeyOrName(panelDashboardControls, "cursorcontroller", "CursorControlUserControl");
            ScannerCommon.UserControlManager.StartTopLevelAnimation();
            return success;
        }

        public bool HandleCmdShowSystem()
        {
            ScannerCommon.UserControlManager.PushUserControlByKeyOrName(panelDashboardControls, "supportedapps", "LaunchAppUserControl");

            if (_launchAppAgent != null) 
            {
                Control[] uc = panelDashboardControls.Controls.Find("LaunchAppUserControl", true);
                _launchAppAgent?.Activate(uc.Length > 0 ? uc[0] as IUserControl : null);
            }

            ScannerCommon.UserControlManager.StartTopLevelAnimation();
            return true;
        }

        public bool HandleCmdLaunchBrowser(string Command, object source)
        {
            LargeToolbarUserControl largeToolbarUserControl = source as LargeToolbarUserControl;
            largeToolbarUserControl?.OnButtonClicked(source, new DashboardAppCommandHandler.CommandHandlerArgs(Command));
            
            return true;
        }

        public bool ShowTalkPanel(WordPredictionModes mode)
        {
            var startupArg = new StartupArg("TalkApplicationScanner")
            {
                QuitAppOnFormClose = false,
                PredictionMode = mode
            };

            // CQRS: Use command handler instead of direct singleton access
            var createPanelHandler = Context.ServiceProvider?.GetService(typeof(ICommandHandler<CreatePanelCommand>)) as ICommandHandler<CreatePanelCommand>;
            Form form;
            if (createPanelHandler != null)
            {
                var command = new CreatePanelCommand("TalkApplicationScanner", startupArg: startupArg);
                createPanelHandler.Handle(command);
                form = command.CreatedPanel as Form;
            }
            else
            {
                form = PanelManager.Instance.CreatePanel("TalkApplicationScanner", startupArg);
            }
            if (form == null)
            {
                _logger.LogError("Could not create TalkApplicationScanner panel.");
                return false;
            }

            IApplicationAgent agent = Context.AppAgentMgr.GetAgentByName("Talk Application Agent");
            if (agent == null)
            {
                _logger.LogError("Talk Application Agent not found. Ensure it is registered in the ACAT configuration.");
                form.Dispose();
                return false;
            }

            // Register the agent safely with the form handle
            Context.AppAgentMgr.AddAgent(form.Handle, agent);
            
            try
            {
                Context.AppPanelManager.ShowDialog(this, form as IPanel);
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing talk panel");
                return false;
            }
            //finally
            //{
            //    // Unregister the agent safely
            //    Context.AppAgentMgr.RemoveAgent(agent.Name);
            //    //Context.AppAgentMgr.RemoveAgentByAgent(agent);
            //    form.Dispose();
            //}

        }


        public bool HandleTalkAppRequest(string Command)
        {
            _logger.LogDebug("Handling Talk App Request: {Command}", Command);
            ScannerCommon.UserControlManager.StopTopLevelAnimation();
            this.Hide();

            WordPredictionModes predictionMode = WordPredictionModes.None;
            if (Command == "CmdTalkSentenceMode")
            {
                predictionMode = WordPredictionModes.Sentence;
            }
            else if (Command == "CmdTalkPhraseMode")
            {
                predictionMode = WordPredictionModes.CannedPhrases;
            }
            else if (Command == "CmdTalkShorthandMode")
            {
                predictionMode = WordPredictionModes.Shorthand;
            }

            else
            {
                _logger.LogError("Unknown command: {Command}", Command);
                return false;
            }

            var success = ShowTalkPanel(predictionMode);

            this.Show();
            ScannerCommon.UserControlManager.StartTopLevelAnimation();
            return success;
        }

        //public bool HandleSwitchWindowRequest()
        //{
        //    var switchWindowAgent = Context.AppAgentMgr.GetFunctionalAgentByName("Windows Task Switcher") as IFunctionalAgent;
        //    if (switchWindowAgent != null)
        //    {
        //        Log.Debug("Switching window using SwitchWindowAgent.");
        //        switchWindowAgent.Activate();
        //        return true;
        //    }
        //    else
        //    {
        //        Log.Error("SwitchWindowAgent not found. Ensure it is registered in the ACAT configuration.");
        //        return false;
        //    }
        //}

        private void InitializeDashboard()
        {
            this.panelDashboardControls.Name = "Dashboard";
            this.panelDashboardControls.AccessibleName = "Dashboard";
            this.panelDashboardControls.Dock = DockStyle.Top;
            this.panelDashboardControls.AutoSize = true;
            this.panelDashboardControls.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void InitializeTopToolbar()
        {
            this.panelTopToolbar.Name = "Toolbar";
            this.panelTopToolbar.AccessibleName = "Toolbar";
            this.panelTopToolbar.Dock = DockStyle.Top;
            this.panelTopToolbar.AutoSize = true;
            this.panelTopToolbar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
        protected class DashboardAppCommandHandler : RunCommandHandler
        {
            private readonly ILogger<DashboardAppScanner> _logger;

            public class CommandHandlerArgs :EventArgs
            {
                public CommandHandlerArgs(String command)
                {
                    Command = command;
                }
                public String Command { get; }

                public override string ToString()
                {
                    return Command;
                }
            }

            public DashboardAppCommandHandler(String cmd) : base(cmd) 
            {
                _logger = LoggingConfiguration.CreateLogger<DashboardAppScanner>();
            }

            public override bool Execute(ref bool handled)
            {
                _logger.LogInformation("Executing command: {Command}", Command);
                var form = Dispatcher.Scanner.Form as DashboardAppScanner;

                handled = Command switch
                {
                    "CmdTalkSentenceMode" => form.HandleTalkAppRequest(Command),
                    "CmdTalkPhraseMode" => form.HandleTalkAppRequest(Command),
                    "CmdTalkShorthandMode" => form.HandleTalkAppRequest(Command),
                    "Exit" => form.HandleCmdExit(),
                    "CmdGoBack" => form.HandleCmdGoBack(),
                    _ => false,
                };

                return true;
            }

            public override bool Execute(ref bool handled, object source = null)
            {
                var form = Dispatcher.Scanner.Form as DashboardAppScanner;

                form.ScannerCommon.UserControlManager.StopTopLevelAnimation();
                form.Visible = false;

                form._logger.LogInformation("Executing command: {Command} from source: {SourceType}", Command, source?.GetType().Name);

                switch (Command)
                {
                    case "Chrome":
                    case "Edge":
                        form.HandleCmdLaunchBrowser(Command, source);
                        break;
                }
                handled = true;

                form.Visible = true;
                form.ScannerCommon.UserControlManager.StartTopLevelAnimation();
                return true;
            }
        }

        private bool HandleCmdExit()
        {
            throw new NotImplementedException();
        }

        private bool HandleCmdShowKeyboard()
        {
            ScannerCommon.UserControlManager.PushUserControlByKeyOrName(panelDashboardControls, "keyboard", "KeyboardUserControl");
            ScannerCommon.UserControlManager.StartTopLevelAnimation();
            return true;
        }

        private bool HandleCmdGoBack()
        {
            ScannerCommon.UserControlManager.PopUserControl(panelDashboardControls);
            ScannerCommon.UserControlManager.StartTopLevelAnimation();
            return true;
        }

        private class DashboardAppDispatcher : DefaultCommandDispatcher
        {

            public DashboardAppDispatcher(IScannerPanel panel) : base(panel)
            {
                /* Main Menu Commands */
                Commands.Add(new DashboardAppCommandHandler("CmdTalkSentenceMode"));
                Commands.Add(new DashboardAppCommandHandler("CmdTalkPhraseMode"));
                Commands.Add(new DashboardAppCommandHandler("CmdTalkShorthandMode"));
            }
        }
    }
}
