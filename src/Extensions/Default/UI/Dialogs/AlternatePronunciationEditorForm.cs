using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Permissions;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aster.WidgetManagement;
using Aster.Utility;
using System.Threading;
using System.IO;
using System.Xml;
using Aster.AnimationManagement;
using Aster.InputActuators;
using Aster.ScreenManagement;
using Aster.ExtensionHelper;
using Aster.Widgets;
using Aster.AbbreviationsManagement;

namespace Aster.Extensions.Base.UI.Dialogs
{
    public partial class AlternatePronunciationEditorForm : Form, IDialogPanel
    {
        //bool isDirty = false;

        public String _originalTerm;
        public String _replacementTerm;
        public Boolean _hasTerm;

        private DialogCommon _dialogCommon;
        private WindowActiveWatchdog _windowActiveWatchdog;
        public AlternatePronunciationEditorForm()
        {
            InitializeComponent();
            init(String.Empty, String.Empty);
        }

        public AlternatePronunciationEditorForm(String originalTerm, String replacementTerm)
        {
            InitializeComponent();
            init(originalTerm, replacementTerm);
        }

        void init(String originalTerm, String replacementTerm)
        {
            _dialogCommon = new DialogCommon(this);

            // _windowActiveWatchdog = new WindowActiveWatchdog(this);

            _originalTerm = originalTerm;
            _replacementTerm = replacementTerm;
            _hasTerm = false;

            if (!_dialogCommon.Initialize())
            {
                MessageBox.Show("Initialization error");
            }

            this.Load += new EventHandler(AsterScreenTemplateForm_Load);
            this.FormClosing += new FormClosingEventHandler(AsterScreenTemplateForm_FormClosing);
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        void AsterScreenTemplateForm_Load(object sender, EventArgs e)
        {
            _windowActiveWatchdog = new WindowActiveWatchdog(this);
            initWidgetSettings();

            _dialogCommon.OnLoad();
            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        void AsterScreenTemplateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _windowActiveWatchdog.Dispose();
            _dialogCommon.OnClosing();
        }

        void widget_EvtValueChanged(Widget widget)
        {
            //isDirty = true;
        }

        void finish()
        {
            if (String.IsNullOrEmpty(tbOriginalTerm.Text))
            {
                // prompt user that pronunciation cannot be blank!
                DialogUtils.ShowTimedDialog(this, "Error", "Please enter a term!");
            }
            else
            {
                _originalTerm = tbOriginalTerm.Text;
                _replacementTerm = tbReplacementTerm.Text;

                Widget rootWidget = _dialogCommon.GetRootWidget();

                _hasTerm = true;
                Windows.CloseForm(this);
            }
        }

        void undo()
        {
            bool undo = true;

            if (!DialogUtils.Confirm(this, "Undo Change?"))
            {
                undo = false;
            }

            if (undo)
            {
                // use the init call to repopulate the screen with original settings
                initWidgetSettings();
            }
        }

        void cancel()
        {
            Log.Debug();
            /*
            bool cancel = true;

            if (!ScreenUtility.Confirm(this, "Are you sure you want to quit?"))
            {
                cancel = false;
            }

            if (cancel)*/
            {
                Invoke(new MethodInvoker(delegate()
                {
                    _hasTerm = false;
                    Windows.CloseForm(this);
                }));
            }
        }

        /// <summary>
        /// Triggered when a widget is triggered
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            String value = widget.Value;

            if (String.IsNullOrEmpty(value))
            {
                Log.Fatal("OnButtonActuated() -- received actuation from empty widget!");
                return;
            }

            Invoke(new MethodInvoker(delegate()
                {
                    switch (value)
                    {
                        case "valButtonFinished":
                            finish();
                            break;

                        case "valButtonUndo":
                            undo();
                            break;

                        case "valButtonCancel":
                            cancel();
                            break;

                        default:
                            // TODO add default case?
                            //Log.Fatal("OnButtonActuated() -- unhandled widget actuation!");
                            break;
                    }
                }));
        }

        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        public void OnResume()
        {
            _dialogCommon.OnResume();
        }

        public Control GetTitle()
        {
            return Title;
        }

        public void OnRunCommand(string command, ref bool handled)
        {
            handled = true;

            switch (command)
            {
                default:
                    handled = false;
                    break;
            }
        }

        void initWidgetSettings()
        {
            Widget rootWidget = _dialogCommon.GetRootWidget();

            // TODO see if we can rely on the states being correct or if we need to force
            // only one to be true

            Windows.SetText(tbOriginalTerm, _originalTerm);
            Windows.SetText(tbReplacementTerm, _replacementTerm);
        }

        void RadioSetTypeChoice( String widgetName, Boolean choice)
        {
            // set all to false and then just reset the passed-in one to true
            Widget rootWidget = _dialogCommon.GetRootWidget();

            WidgetUtils.SetToggleLabelButtonState(rootWidget, widgetName, choice);        
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }
    }
}
