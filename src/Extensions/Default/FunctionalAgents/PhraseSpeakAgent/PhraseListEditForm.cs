using ACAT.ACATResources;
using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.PhraseSpeakAgent
{
    [DescriptorAttribute("4308B7FF-FEE5-4E03-AB16-7BCB54C923AA",
                            "PhraseListEditForm",
                            "Phrase list editor")]
    public partial class PhraseListEditForm : Form, IDialogPanel, IExtension
    {
        public bool Cancel;

        /// <summary>
        /// Extension invoker object to invoke properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// The dialog common object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        private Phrases _phrases;

        private bool _isDirty = false;

        /// <summary>
        /// Makes sure this window stays active and keeps focus
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        public PhraseListEditForm()
        {
            InitializeComponent();
            _invoker = new ExtensionInvoker(this);
            Load += PhraseListEditForm_Load;
            FormClosing += PhraseListEditForm_FormClosing;
        }

        private enum Flags
        {
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _dialogCommon; } }

        /// <summary>
        /// Gets the sync object used for synchronization
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the style of the form
        /// </summary>
        protected override CreateParams CreateParams
        {
            get { return DialogCommon.SetFormStyles(base.CreateParams); }
        }

        public void ExtendedKeyDown(Keys key)
        {
            User32Interop.keybd_event((byte)key, 0xAA, 0, UIntPtr.Zero);
        }

        public void ExtendedKeyUp(Keys key)
        {
            User32Interop.keybd_event((byte)key, 0xAA, (uint)Flags.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        /// <summary>
        /// Returns the extension invoker
        /// </summary>
        /// <returns>the invoker</returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Intitializes the class
        /// </summary>
        /// <param name="startupArg">startup param</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _dialogCommon = new DialogCommon(this);

            return _dialogCommon.Initialize(startupArg);
        }

        /// <summary>
        /// Invoked when the user selects a widget
        /// in the dialog such as a button or a text box.
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            String value = widget.Value;

            if (String.IsNullOrEmpty(value))
            {
                return;
            }

            Invoke(new MethodInvoker(delegate
            {
                switch (value)
                {
                    case "buttonOK":
                        finish();
                        break;

                    case "buttonCancel":
                        cancel();
                        break;

                    case "buttonUp":
                        handleUp();
                        break;

                    case "buttonDown":
                        handleDown();
                        break;

                    case "buttonPageUp":
                        handlePageUp();
                        break;

                    case "buttonPageDown":
                        handlePageDown();
                        break;

                    case "buttonMoveUp":
                        handleMoveUp();
                        break;

                    case "buttonMoveDown":
                        handleMoveDown();
                        break;

                    case "buttonEdit":
                        handleEdit();
                        break;

                    case "buttonDelete":
                        handleDelete();
                        break;

                    case "buttonAdd":
                        handleAdd();
                        break;

                    case "buttonInsert":
                        handleInsert();
                        break;
                }
            }));
        }

        /// <summary>
        /// Pauses the animation
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public void OnResume()
        {
            _dialogCommon.OnResume();
        }

        /// <summary>
        /// Invoked when there is a need to run a command
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="handled">was it handled</param>
        public void OnRunCommand(string command, ref bool handled)
        {
            switch (command)
            {
                default:
                    handled = false;
                    break;
            }
        }

        /// <summary>
        /// Client size changed
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if (_firstClientChangedCall)
            {
                _designTimeAspectRatio = (float)ClientSize.Height / ClientSize.Width;
                _firstClientChangedCall = false;
            }
        }

        /// <summary>
        /// Windows procedure
        /// </summary>
        /// <param name="m">windows message</param>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// User wants to cancel out of the dialog box
        /// </summary>
        private void cancel()
        {
            Log.Debug();

            bool quit = !_isDirty || DialogUtils.Confirm(R.GetString("CancelAndQuit"));
            if (quit)
            {
                Invoke(new MethodInvoker(delegate
                {
                    Cancel = true;
                    Windows.CloseForm(this);
                }));
            }
        }

        /// <summary>
        /// User is done. Confirm, perform validation and check
        /// that everything is OK and then quit.
        /// </summary>
        private void finish()
        {
            var quit = !_isDirty || DialogUtils.Confirm(R.GetString("SaveAndQuit"));
            if (quit)
            {
                Cancel = false;
                updateAndSaveList();
                Windows.CloseForm(this);
            }
        }

        private String formatText(Phrase phrase)
        {
            return phrase.Favorite ? phrase.Text + "*" : phrase.Text;
        }

        private void handleAdd()
        {
            var dlg = Context.AppPanelManager.CreatePanel("PhraseEditorForm");
            var form = dlg as PhraseEditorForm;
            Hide();
            Context.AppPanelManager.ShowDialog(Context.AppPanelManager.GetCurrentPanel(), dlg as IPanel);
            Show();

            if (!form.Cancel)
            {
                _isDirty = true;
                phraseAdd(form.Phrase);
            }
        }

        private void handleDelete()
        {
            int selectedIndex = listBoxPhrases.SelectedIndex;
            if (selectedIndex >= 0)
            {
                var phrase = listBoxPhrases.Items[selectedIndex] as string;

                var trimLength = 20;
                var text = (phrase.Length > trimLength) ? (phrase.Substring(0, trimLength) + "...") : phrase;

                var prompt = String.Format(R.GetString("DeleteAbbr"), "\"" + text + "\"");

                if (!DialogUtils.Confirm(prompt))
                {
                    return;
                }

                listBoxPhrases.Items.RemoveAt(selectedIndex);
                _isDirty = true;

                if (listBoxPhrases.Items.Count > 0)
                {
                    if (selectedIndex >= listBoxPhrases.Items.Count)
                        listBoxPhrases.SelectedIndex = listBoxPhrases.Items.Count - 1;
                    else
                        listBoxPhrases.SelectedIndex = selectedIndex;
                }
            }
        }

        private void handleDown()
        {
            listBoxPhrases.Focus();
            ExtendedKeyDown(Keys.Down);
            ExtendedKeyUp(Keys.Down);
        }

        private void handleEdit()
        {
            var index = listBoxPhrases.SelectedIndex;
            if (index >= 0)
            {
                var dlg = Context.AppPanelManager.CreatePanel("PhraseEditorForm");
                var form = dlg as PhraseEditorForm;
                if (form != null)
                {
                    var text = listBoxPhrases.Items[index] as String;
                    form.Phrase = new Phrase(unformatText(text), isFavorite(text));
                }

                Hide();
                Context.AppPanelManager.ShowDialog(Context.AppPanelManager.GetCurrentPanel(), dlg as IPanel);
                Show();

                if (!form.Cancel)
                {
                    _isDirty = true;
                    phraseUpdate(index, form.Phrase);
                }
            }
        }

        private void handleInsert()
        {
            if (listBoxPhrases.Items.Count == 0)
            {
                handleAdd();
                return;
            }

            var index = listBoxPhrases.SelectedIndex;
            if (index >= 0)
            {
                var dlg = Context.AppPanelManager.CreatePanel("PhraseEditorForm");
                var form = dlg as PhraseEditorForm;

                Hide();
                Context.AppPanelManager.ShowDialog(Context.AppPanelManager.GetCurrentPanel(), dlg as IPanel);
                Show();

                if (!form.Cancel)
                {
                    _isDirty = true;
                    phraseInsert(index, form.Phrase);
                }
            }
        }

        private void handleMoveDown()
        {
            int selectedIndex = listBoxPhrases.SelectedIndex;
            if (selectedIndex < listBoxPhrases.Items.Count - 1 & selectedIndex != -1)
            {
                _isDirty = true;

                listBoxPhrases.Items.Insert(selectedIndex + 2, listBoxPhrases.Items[selectedIndex]);
                listBoxPhrases.Items.RemoveAt(selectedIndex);
                listBoxPhrases.SelectedIndex = selectedIndex + 1;
            }
        }

        private void handleMoveUp()
        {
            int selectedIndex = listBoxPhrases.SelectedIndex;
            if (selectedIndex > 0)
            {
                _isDirty = true;

                listBoxPhrases.Items.Insert(selectedIndex - 1, listBoxPhrases.Items[selectedIndex]);
                listBoxPhrases.Items.RemoveAt(selectedIndex + 1);
                listBoxPhrases.SelectedIndex = selectedIndex - 1;
            }
        }

        private void handlePageDown()
        {
            listBoxPhrases.Focus();
            ExtendedKeyDown(Keys.PageDown);
            ExtendedKeyUp(Keys.PageDown);
        }

        private void handlePageUp()
        {
            listBoxPhrases.Focus();
            ExtendedKeyDown(Keys.PageUp);
            ExtendedKeyUp(Keys.PageUp);
        }

        private void handleUp()
        {
            listBoxPhrases.Focus();
            ExtendedKeyDown(Keys.Up);
            ExtendedKeyUp(Keys.Up);
        }

        private bool isFavorite(String phraseText)
        {
            if (phraseText.Length > 1)
            {
                return phraseText.EndsWith("*");
            }

            return false;
        }

        private void phraseAdd(Phrase phrase)
        {
            listBoxPhrases.SelectedIndex = listBoxPhrases.Items.Add(formatText(phrase));
        }

        private void phraseInsert(int index, Phrase phrase)
        {
            var text = formatText(phrase);
            listBoxPhrases.Items.Insert(index, text);
            index = listBoxPhrases.Items.IndexOf(text);
            listBoxPhrases.SelectedIndex = index;
        }

        private void PhraseListEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _windowActiveWatchdog.Dispose();

            _dialogCommon.OnClosing();
        }

        private void PhraseListEditForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            Text = R.GetString("EditPhrases");

            labelFavoritePhrase.Text = "* " + R.GetString("FavoritePhrase");

            _phrases = Phrases.Load();

            foreach (var phrase in _phrases.PhraseList)
            {
                phraseAdd(phrase);
            }

            listBoxPhrases.Focus();
            if (listBoxPhrases.Items.Count > 0)
            {
                listBoxPhrases.SelectedIndex = 0;
            }

            _windowActiveWatchdog = new WindowActiveWatchdog(this);

            _dialogCommon.OnLoad();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        private void phraseUpdate(int index, Phrase phrase)
        {
            if (index >= 0)
            {
                listBoxPhrases.Items[index] = formatText(phrase);
            }
        }

        private String unformatText(String phraseText)
        {
            if (phraseText.Length > 1)
            {
                return phraseText.EndsWith("*") ? phraseText.Substring(0, phraseText.Length - 1) : phraseText;
            }

            return phraseText;
        }

        private void updateAndSaveList()
        {
            var phrases = new List<Phrase>();

            foreach (var item in listBoxPhrases.Items)
            {
                var t = unformatText(item as String);
                phrases.Add(new Phrase(unformatText(item as String), isFavorite(item as String)));
            }

            _phrases.PhraseList = phrases;

            _phrases.Save();
        }
    }
}