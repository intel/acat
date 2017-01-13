////////////////////////////////////////////////////////////////////////////
// <copyright file="ThemeSelectForm.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Lib.Core.ThemeManagement
{
    /// <summary>
    /// Displays a list of Themes (color schemes)
    /// and lets the user select a theme from the list
    /// </summary>
    public partial class ThemeSelectForm : Form
    {
        /// <summary>
        /// Theme manager singleton
        /// </summary>
        private ThemeManager _themeManager;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public ThemeSelectForm()
        {
            InitializeComponent();

            Load += ThemeSelectForm_Load;
            FormClosing += ThemeSelectForm_FormClosing;
        }

        /// <summary>
        /// Gets the theme the user selected
        /// </summary>
        public String SelectedTheme { get; private set; }

        /// <summary>
        /// Displays the theme select form to enable the user to select
        /// the preferred theme
        /// </summary>
        /// <returns>The selected culture</returns>
        public static String SelectTheme()
        {
            var form = new ThemeSelectForm();
            if (form.ShowDialog() == DialogResult.Cancel)
            {
                return String.Empty;
            }

            return form.SelectedTheme;
        }

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// User clicked cancel
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
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
        /// Selection changed. Update label and preview image
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void listBoxLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatePreviewImage();
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
        /// User selected something.  Save the selection and
        /// close the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void listBoxThemes_DoubleClick(object sender, EventArgs e)
        {
            selectAndClose();
        }

        /// <summary>
        /// Set the selected language and close the form
        /// </summary>
        private void selectAndClose()
        {
            setSelectedItem();
            MessageBox.Show("Color scheme selected: " + SelectedTheme, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Set the selected theme
        /// </summary>
        private void setSelectedItem()
        {
            if (comboBoxThemes.SelectedIndex >= 0)
            {
                SelectedTheme = comboBoxThemes.SelectedItem as String;
            }
        }

        private void ThemeSelectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_themeManager != null)
            {
                _themeManager.Dispose();
            }
        }

        /// <summary>
        /// Form loader. Display list of themes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ThemeSelectForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            TopMost = true;

            CenterToScreen();

            _themeManager = ThemeManager.Instance;

            _themeManager.Init();

            var themes = _themeManager.Themes;
            if (!themes.Any())
            {
                MessageBox.Show("No themes found", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            foreach (var theme in themes)
            {
                comboBoxThemes.Items.Add(theme);
            }

            comboBoxThemes.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxThemes.SelectedIndex = 0;

            updatePreviewImage();

            comboBoxThemes.SelectedIndexChanged += listBoxLanguages_SelectedIndexChanged;
        }

        /// <summary>
        /// Displays the preview image correponding to the
        /// currently selected theme
        /// </summary>
        private void updatePreviewImage()
        {
            if (comboBoxThemes.SelectedIndex >= 0)
            {
                var item = comboBoxThemes.SelectedItem;
                var themeDir = _themeManager.GetThemeDir(item as String);
                var path = Path.Combine(themeDir, Theme.PreviewScannerImageName);
                if (File.Exists(path))
                {
                    var img = Image.FromFile(path);
                    if (img != null)
                    {
                        float widthScale = (float) pictureBoxPreview.Width/img.Width;
                        float heightScale = (float) pictureBoxPreview.Height/img.Height;
                        float min = Math.Min(widthScale, heightScale);

                        img = ImageUtils.ImageResize(img, (int)(img.Width*min), (int)(img.Height*min));
                        pictureBoxPreview.BackgroundImage = img;
                        pictureBoxPreview.BackgroundImageLayout = ImageLayout.Center;
                    }
                }
            }
        }
    }
}