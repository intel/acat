////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Displays a list of languages (culturues, localized resource folders)
    /// discovered to let the user select a language from the list
    /// </summary>
    public partial class LanguageSelectForm : Form
    {
        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public LanguageSelectForm()
        {
            InitializeComponent();

            Load += LanguageSelectForm_Load;
        }

        /// <summary>
        /// Gets whether the selected language chosen as the default
        /// </summary>
        public static bool IsDefault { get; private set; }

        /// <summary>
        /// Gets the culture the user selected
        /// </summary>
        public CultureInfo SelectedCulture { get; private set; }

        /// <summary>
        /// Displays the language form to enable the user to select
        /// the preferred language
        /// </summary>
        /// <returns>The selected culture</returns>
        public static CultureInfo SelectLanguage()
        {
            var cultureInfos = ResourceUtils.EnumerateInstalledLanguages();

            if (cultureInfos.Count() == 1)
            {
                return cultureInfos.ElementAt(0);
            }

            var form = new LanguageSelectForm();
            if (form.ShowDialog() == DialogResult.Cancel || form.SelectedCulture == null)
            {
                form.Dispose();
                return null;
            }

            form.Dispose();

            IsDefault = form.checkBoxSetAsDefault.Checked;

            return form.SelectedCulture;
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
        /// User clicked cancel
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            SelectedCulture = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// User selected a culture.  Close the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            selectAndClose();
        }

        /// <summary>
        /// Returns the index of the culture in the listview that
        /// corresponds to the current culture.  -1 if culture is not found
        /// </summary>
        /// <returns>index</returns>
        private int getCurrentLanguageIndex()
        {
            if (listBoxLanguages.Items.Count == 0)
            {
                return -1;
            }

            // no culture is set, set it to "en"
            if (CultureInfo.DefaultThreadCurrentUICulture == null)
            {
                foreach (ListViewItem item in listBoxLanguages.Items)
                {
                    var culture = item.Tag as CultureInfo;
                    if (String.Compare(culture.TwoLetterISOLanguageName, "en", true) == 0 &&
                        (String.Compare(culture.Name, "en", true) == 0 ||
                         String.Compare(culture.Name, "en-US", true) == 0))
                    {
                        return item.Index;
                    }
                }

                return 0;
            }

            foreach (ListViewItem item in listBoxLanguages.Items)
            {
                var culture = item.Tag as CultureInfo;
                if (String.Compare(culture.Name, CultureInfo.DefaultThreadCurrentUICulture.Name, true) == 0)
                {
                    return item.Index;
                }
            }

            foreach (ListViewItem item in listBoxLanguages.Items)
            {
                var culture = item.Tag as CultureInfo;

                if (String.Compare(culture.TwoLetterISOLanguageName,
                        CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, true) == 0)
                {
                    return item.Index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Form loader. Enumerate the cultures and display the
        /// names in a list
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void LanguageSelectForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            CenterToScreen();

            TopMost = false;
            TopMost = true;

            labelCurrentLanguage.Text = (CultureInfo.DefaultThreadCurrentUICulture != null)
                ? CultureInfo.DefaultThreadCurrentUICulture.DisplayName
                : "[Not set]";

            listBoxLanguages.MultiSelect = false;
            listBoxLanguages.View = View.List;

            listBoxLanguages.DoubleClick += listBoxLanguages_DoubleClick;
            var cultureInfos = ResourceUtils.EnumerateInstalledLanguages();

            checkBoxSetAsDefault.CheckState = CheckState.Checked;

            foreach (var culture in cultureInfos)
            {
                var text = culture.DisplayName + " (" + culture.TwoLetterISOLanguageName + ")";
                listBoxLanguages.Items.Add(new ListViewItem(text) { Tag = culture });
            }

            if (listBoxLanguages.Items.Count > 0)
            {
                int index = getCurrentLanguageIndex();

                if (index < 0)
                {
                    index = 0;
                }

                listBoxLanguages.SelectedIndices.Add(index);
            }

            listBoxLanguages.HideSelection = false;
        }

        /// <summary>
        /// User selected something.  Save the selection and
        /// close the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void listBoxLanguages_DoubleClick(object sender, EventArgs e)
        {
            selectAndClose();
        }

        /// <summary>
        /// Set the selected language and close the form
        /// </summary>
        private void selectAndClose()
        {
            setSelectedItem();
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Set the selected culture
        /// </summary>
        private void setSelectedItem()
        {
            if (listBoxLanguages.SelectedItems.Count > 0)
            {
                ListViewItem item = listBoxLanguages.SelectedItems[0];
                SelectedCulture = item.Tag as CultureInfo;
            }
        }
    }
}