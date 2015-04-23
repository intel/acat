using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Windows.Forms;
using Aster.WidgetManagement;
using Aster.Utility;
using System.Threading;
using System.IO;
using System.Xml;
using Aster.Extensions.Base.UI.Dialogs;
using Aster.AnimationManagement;
using Aster.InputActuators;
using Aster.ScreenManagement;
using Aster.ExtensionHelper;
using Aster.AbbreviationsManagement;

namespace Aster.Extensions.Base.UI.Dialogs
{
    public partial class AlternatePronunciationDataForm : Form, IDialogPanel
    {
        delegate bool exportPronunciationFromListView(ListView listview);
        delegate bool addRowToListView(ListView listview, int columns, String[] str);
        delegate bool importPronunciationIntoListView(ListView listview, Boolean clearFirst);

        // AP = alternative pronunciation
        private const int AP_COLUMN_COUNT = 2;
        private const int COLUMN_TERM = 0;
        private const int COLUMN_ALTERNATE = 1;

        private volatile bool _sync = false;

        private DialogCommon _dialogCommon;
        bool isDirty = false;

        bool _hideForm = false;

        private WindowActiveWatchdog _windowActiveWatchdog;

        public AlternatePronunciationDataForm()
        {
            InitializeComponent();  

            _dialogCommon = new DialogCommon(this);

            if (!_dialogCommon.Initialize())
            {
                MessageBox.Show("Initialization error");
            }

            this.KeyPreview = true;
            listViewAP.KeyPress += new KeyPressEventHandler(listViewAP_KeyPress);
            this.Load += new EventHandler(AlternatePronunciationDataForm_Load);
            this.FormClosing += new FormClosingEventHandler(AlternatePronunciationDataForm_FormClosing);
        }

        void listViewAP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x0D)
            {
                e.Handled = true;
                this.BeginInvoke(new MethodInvoker(delegate()
                {
                    edit();
                }));
            }
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        void AlternatePronunciationDataForm_Load(object sender, EventArgs e)
        {
            Log.Debug("Entering...");
            _windowActiveWatchdog = new WindowActiveWatchdog(this);

            _dialogCommon.OnLoad();

            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());

            // populate listview from xml config file
            ImportPronunciationIntoListView(listViewAP, true);

            // select the first item by default
            const int DEFAULT_INITIAL_LISTVIEW_POSITION = 0;
            if (listViewAP.Items.Count > 0)
            {
                listViewAP.Items[DEFAULT_INITIAL_LISTVIEW_POSITION].Selected = true; // DEFAULT_INITIAL_LISTVIEW_POSITION;
            }
            else
            {
                Log.Debug("did not load any items!");
            }

            listViewAP.AllowColumnReorder = false;
            listViewAP.AllowDrop = false;
            listViewAP.MultiSelect = false;
            listViewAP.FullRowSelect = true;

            setButtonStates();
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        void AlternatePronunciationDataForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            Invoke(new MethodInvoker(delegate()
            {
                if (_windowActiveWatchdog != null)
                {
                    _windowActiveWatchdog.Dispose();
                }
            }));
            */

            _windowActiveWatchdog.Dispose();
            _dialogCommon.OnClosing();
        }

        void ListViewChanged()
        {
            isDirty = true;
            setButtonStates();
        }

        void setButtonStates()
        {
            if (listViewAP.Items.Count > 0)
            {
                lblEdit.Enabled = true;
                lblDelete.Enabled = true;
            }
            else
            {
                lblEdit.Enabled = false;
                lblDelete.Enabled = false;
            }
        }

        void add()
        {
            Boolean isCompleted = false;
            String originalTerm = String.Empty;
            String replacementTerm = String.Empty;

            do
            {
                Invoke(new MethodInvoker(delegate()
                {
                    _hideForm = true;
                    AlternatePronunciationEditorForm dlg = new AlternatePronunciationEditorForm(originalTerm, replacementTerm);
                    Context.AppScreenManager.ShowDialog(this, dlg);

                    if (dlg._hasTerm == false)
                    {
                        isCompleted = true;
                        return; // user must have cancelled
                    }

                    if (String.IsNullOrEmpty(dlg._originalTerm))
                    {
                        isCompleted = true;
                        return; // this should have already been validated in the editor dialog
                    }

                    Log.Debug("originalterm=" + dlg._originalTerm + " replacementterm=" + dlg._replacementTerm);

                    originalTerm = dlg._originalTerm;
                    replacementTerm = dlg._replacementTerm;

                    // check if pronunciation already exists
                    if (Context.AppPronunications.Exists(dlg._originalTerm))
                    {
                        // prompt user that pronunciation already exists!
                        DialogUtils.ShowTimedDialog(this, "Error", "Pronunciation for '" + dlg._originalTerm + "' already exists");
                    }
                    else
                    {
                        AddPronunciation(listViewAP, dlg._originalTerm, dlg._replacementTerm);
                        isCompleted = true;
                        ListViewChanged();
                        return;
                    }
                }));
            } while (!isCompleted);          
        }

        void delete()
        {
            bool delete = true;

            Invoke(new MethodInvoker(delegate()
            {
                String[] itemCells;
                itemCells = new String[AP_COLUMN_COUNT];

                Boolean gotListViewItem = Windows.GetSelectedListViewItem(listViewAP, ref itemCells);
                String term = itemCells[COLUMN_TERM].ToString();

                if (!DialogUtils.Confirm(this, "Delete " + term + "?"))
                {
                    delete = false;
                }

                if (delete)
                {
                    int index = 0;
                    Invoke(new MethodInvoker(delegate()
                    {
                        ListView.SelectedIndexCollection coll = listViewAP.SelectedIndices;
                        if (coll.Count > 0)
                        {
                            index = coll[0];
                        }
                    }));

                    Context.AppPronunications.Remove(term);
                    ImportPronunciationIntoListView(this.listViewAP);

                    if (listViewAP.Items.Count > 0)
                    {
                        Invoke(new MethodInvoker(delegate()
                        {
                            if (index >= listViewAP.Items.Count)
                            {
                                index = 0;
                            }
                            listViewAP.Items[index].Selected = true;
                            listViewAP.Items[index].EnsureVisible();
                        }));
                    }

                    ListViewChanged();
                }
            }));
        }

        void edit()
        {
            Boolean isCompleted = false;
            String[] itemCells;
            itemCells = new String[AP_COLUMN_COUNT];

            Boolean gotListViewItem = Windows.GetSelectedListViewItem(listViewAP, ref itemCells);            

            if (!gotListViewItem)
            {
                Log.Fatal(" No item selected to edit!");
                return;
            }

            Log.Debug("itemCells[COLUMN_TERM]=" + itemCells[COLUMN_TERM]);

            do
            {
                Invoke(new MethodInvoker(delegate()
                {
                    String oldTerm = itemCells[COLUMN_TERM].ToString();

                    _hideForm = true;
                    AlternatePronunciationEditorForm dlg = new AlternatePronunciationEditorForm(itemCells[COLUMN_TERM].ToString(),
                                                                    itemCells[COLUMN_ALTERNATE].ToString());
                    Context.AppScreenManager.ShowDialog(this, dlg);

                    Log.Debug("_originalTerm=" + dlg._originalTerm + "_replacementTerm=" + dlg._replacementTerm);

                    if (dlg._hasTerm == false)
                    {
                        isCompleted = true;
                        return; // user must have cancelled
                    }

                    if (String.IsNullOrEmpty(dlg._originalTerm))
                    {
                        DialogUtils.ShowTimedDialog(this, "Message", "Pronunciation cannot be empty!");
                    }
                    else
                    {
                        UpdatePronunciation(listViewAP, oldTerm, dlg._originalTerm, dlg._replacementTerm);
                        isCompleted = true;
                        ListViewChanged();
                        return;
                    }
                }));
            } while (!isCompleted);
        }

        void quit()
        {
            bool quit = true;

            if (isDirty)
            {
                if (!DialogUtils.Confirm(this, "Changes not saved. Quit?"))
                {
                    quit = false;
                }
            }

            if (quit)
            {
                Invoke(new MethodInvoker(delegate()
                {
                    Windows.CloseForm(this);
                }));
            }
            else
            {
                Context.AppPronunications.Load();
            }
        }

        void save()
        {
            Invoke(new MethodInvoker(delegate()
            {
                if (isDirty)
                {
                    // TODO confirm save?
                    //
                    if (DialogUtils.Confirm(this, "You have made changes.\nSave?"))
                    {
                        Context.AppPronunications.Save();
                        Context.AppPronunications.Load();
                    }
                }

               Windows.CloseForm(this);
            }));
        }

        /// <summary>
        /// Triggered when a widget is triggered
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            Log.Debug("**Actuate** " + widget.UIControl.Name + " Value: " + widget.Value);

            String value = widget.Value;

            Invoke(new MethodInvoker(delegate()
            {
                if (String.IsNullOrEmpty(value))
                {
                    Log.Fatal("OnButtonActuated() -- received actuation from empty widget!");
                    return;
                }

                switch (value)
                {
                    case "valButtonAdd":
                        add();
                        break;

                    case "valButtonDelete":
                        delete();
                        break;

                    case "valButtonEdit":
                        edit();
                        break;

                    case "valButtonCancel":
                        quit();
                        break;

                    case "valButtonOK":
                        save();
                        break;

                    default:
                        break;

                }
            }));
        }

        public void OnPause()
        {
            if (!_sync)
            {
                _sync = true;
                _dialogCommon.OnPause();
                if (_hideForm)
                {
                    Windows.SetVisible(this, false);
                }
                _sync = false;
            }
        }

        public void OnResume()
        {
            if (!_sync)
            {
                _dialogCommon.OnResume();
                if (Windows.GetVisible(this) == false)
                {
                    Windows.SetVisible(this, true);
                }
                _hideForm = false;
            }
        }

        public Control GetTitle()
        {
            return Title;
        }

        public void OnRunCommand(string command, ref bool handled)
        {
            handled = false;
        }

        bool UpdatePronunciation(ListView listview, String oldTerm, String newTerm, String replacement)
        {
            bool retVal = true;
            try
            {
                Log.Debug("newTerm=" + newTerm + " replacement=" + replacement);

                if (String.IsNullOrEmpty(newTerm) || String.IsNullOrEmpty(replacement))
                {
                    Log.Debug("Cannot update pronunciation.");
                    return false;
                }

                Context.AppPronunications.Update(oldTerm, new Pronunciation(newTerm, replacement));
                ImportPronunciationIntoListView(listview);
                selectItemInListView(listview, newTerm);
            }
            catch 
            { 
                retVal = false; 
            }

            return retVal;
        }

        bool AddPronunciation(ListView listView, String term, String replacement)
        {
            bool retVal = true;
            try
            {
                if ( String.IsNullOrEmpty(term) || String.IsNullOrEmpty(replacement))
                {
                    Log.Debug("Cannot add pronunciation.");
                    return false;
                }
                
                if (!Context.AppPronunications.Exists(term))
                {
                    Pronunciation pronunciation = new Pronunciation(term, replacement);
                    Context.AppPronunications.Add(pronunciation);
                    ImportPronunciationIntoListView(listView);
                    selectItemInListView(listView, term);
                }
                else
                {
                    retVal = false;
                }
            }

            catch { retVal = false; }
            return retVal;
        }

        void selectItemInListView(ListView listview, string key)
        {
            Invoke(new MethodInvoker(delegate()
            {
                string lookFor = key.ToUpper();
                foreach (ListViewItem item in listview.Items)
                {
                    if (String.Compare(item.Text, key, true) == 0)
                    {
                        item.Selected = true;
                        item.EnsureVisible();
                    }
                }
            }));
        }

        bool ImportPronunciationIntoListView(ListView listview, Boolean clearFirst = true)
        {
            Log.Debug("Entering...");

            if (listview.InvokeRequired)
            {
                return (bool)listview.Invoke(new importPronunciationIntoListView(ImportPronunciationIntoListView), 
                                                new object[] { listview, clearFirst });
            }
            else
            {
                try
                {
                    if (clearFirst)
                    {
                        listview.Items.Clear();
                    }

                    SortedDictionary<String, Pronunciation> pronunciations = Context.AppPronunications.PronunciationList;
                    Log.Debug("PronunciationList size=" + pronunciations.Count);

                    const int PRONUNCIATION_COL_COUNT = 2; // 2 columns in list view, original term and replacement
                    const int TERM_POS = 0;
                    const int REPLACEMENT_POS = 1;

                    foreach(Pronunciation pronunciation in pronunciations.Values)
                    {
                        String[] itemArray = new String[PRONUNCIATION_COL_COUNT];

                        itemArray[TERM_POS] = pronunciation.Word;
                        itemArray[REPLACEMENT_POS] = pronunciation.AltPronunciation;

                        Log.Debug("term=" + itemArray[TERM_POS] + " replacement=" + itemArray[REPLACEMENT_POS]);

                        if (!AddRowToListView(listview, PRONUNCIATION_COL_COUNT, itemArray))
                        {
                            Log.Debug("failed to add item to listview!");
                            throw new SystemException();
                        }
                    }

                    return true;
                }
                catch (IOException ex)
                {
                    // handle this
                    Log.Debug("exception thrown! exception=" + ex.ToString());
                    return false;
                }
            }
        }

        bool AddRowToListView(ListView listview, int columns, String[] cells)
        {
            if (listview.InvokeRequired)
            {
                return (bool)listview.Invoke(new addRowToListView(AddRowToListView), new object[] { listview, columns, cells });
            }
            else
            {
                if (columns != cells.Length)
                {
                    Log.Fatal("Windows.cs::AddRowToListView() - columns/string array size mismatch!");
                    return false;
                }

                ListViewItem lvi = new ListViewItem();

                lvi.Text = cells[0];

                for (int i = 1; i < columns; i++)
                {
                    lvi.SubItems.Add(cells[i]);
                }

                listview.Items.Add(lvi);

                return true;
            }
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

    }
}
