using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using ACAT.Lib.Core.Utility;
using MahApps.Metro.Controls;
using FontStyle = System.Drawing.FontStyle;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms.Integration;
using static ACAT.Lib.Core.Interpreter.Interpret;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Media.Media3D;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using static ACATConfigNext.Program;
using ACAT.Lib.Core.ActuatorManagement;
using System.Speech.Synthesis;
using System.Web.UI.WebControls;


namespace ACATConfigNext
{
    public static class CustomControls
    {
        public static System.Windows.Forms.Label CreateLabel(string text, FontStyle style = FontStyle.Bold)
        {
            return new System.Windows.Forms.Label
            {
                Text = text,
                Dock = DockStyle.Bottom,
                //AutoSize = true,
                Font = new Font("Montserrat", 10, style),
                ForeColor = Color.White,
              //  Margin = new Padding(0, 0, 0, 5)
            };
        }

        public static System.Windows.Forms.Label CreateDescriptionLabel(string description)
        {
            return new System.Windows.Forms.Label
            {
                Text = InsertLineBreaks(description, 60),
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.White,
              //  Margin = new Padding(0, 0, 0, 5)
            };
        }

        private static string InsertLineBreaks(string text, int maxLineLength)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= maxLineLength)
                return text;

            var words = text.Split(' ');
            var result = new List<string>();
            var line = new StringBuilder();

            foreach (var word in words)
            {
                if ((line.Length + word.Length + 1) > maxLineLength)
                {
                    result.Add(line.ToString().TrimEnd());
                    line.Clear();
                }

                line.Append(word + " ");
            }

            if (line.Length > 0)
                result.Add(line.ToString().TrimEnd());

            return string.Join("\n", result);
        }

        public static System.Windows.Forms.TableLayoutPanel CreateCategoryTableLayoutPanel()
        {
            var panel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
              //  Margin = new Padding(10),
                Padding = new Padding(10),
                BackColor = Color.FromArgb(48, 49, 64),
                Dock = DockStyle.Top,
            };

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            return panel;
        }

        public static System.Windows.Forms.CheckBox CreateCheckBox(string text)
        {
            return new System.Windows.Forms.CheckBox
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Montserrat", 8),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Margin = new Padding(5)
            };
        }

        public static System.Windows.Forms.Button CreateFlatButton(string text, object tag = null, int? width = null, int? top = null, int? left = null, int? height = null)
        {
            var button = new System.Windows.Forms.Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                // Font = new Font("Montserrat", 11),
                //ForeColor = Color.White,
                //BackColor = Color.FromArgb(60, 63, 80),
                //Margin = new Padding(5),
                //Padding = new Padding(6),
                //AutoSize = width == null,
                Tag = tag
            };

            if (width.HasValue)
                button.Width = width.Value;

            if (top.HasValue)
                button.Top = top.Value;

            if (left.HasValue)
                button.Left = left.Value;

            if (height.HasValue)
                button.Height = height.Value;

            return button;
        }

        public static System.Windows.Forms.Button CreateSetupButton(string text, EventHandler onClick = null, object tag = null, int? width = null, int? top = null, int? left = null, int? height = null)
        {
            var button = new System.Windows.Forms.Button
            {
                Text = text,
               FlatStyle = FlatStyle.Flat,
               FlatAppearance = { BorderSize = 0 },
                 Font = new Font("Montserrat", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(48, 49, 64),
                Margin = new Padding(5),
                Padding = new Padding(6),
                AutoSize = width == null,
                Tag = tag
            };

             if (width.HasValue)
               button.Width = width.Value;

           if (top.HasValue)
               button.Top = top.Value;

           if (left.HasValue)
               button.Left = left.Value;

         if (height.HasValue)
               button.Height = height.Value;

            if (onClick != null)
            {
                button.Click += onClick;
            }

            return button;
        }


        public static System.Windows.Forms.Panel CreatePanel(DockStyle dock, int width, Color? backColor = null)
        {
            return new System.Windows.Forms.Panel
            {
                Dock = dock,
                Width = width,
                BackColor = backColor ?? Color.Transparent,
                Margin = new Padding(0)
            };
        }

        public static FlowLayoutPanel CreateFlowPanel(DockStyle dock,int height,string text = null,Padding? padding = null,bool autoScroll = true,Color? backColor = null)
        {
            var panel = new FlowLayoutPanel
            {
                Dock = dock,
                Height = height,
                AutoScroll = autoScroll,
                Padding = padding ?? new Padding(5, 5, 0, 0),
                BackColor = backColor ?? Color.Transparent,
                WrapContents = false,
                AutoSize = false,
                Margin = new Padding(0)
            };

            if (!string.IsNullOrEmpty(text))
            {
                var label = new System.Windows.Forms.Label
                {
                    Text = text,
                    AutoSize = true,
                    Font = new Font("Montserrat", 12, FontStyle.Bold),
                    ForeColor = Color.White,
                    Margin = new Padding(0, 5, 10, 0)
                };

                panel.Controls.Add(label);
            }

            return panel;
        }

        public static FrameworkElement CreateLabeledPanel(PropertyInfo prop, object settingsInstance)
        {
            var value = prop.GetValue(settingsInstance);

            //// Horizontal layout for label + input
            //var panel = new StackPanel
            //{
            //    Orientation = Orientation.Horizontal,
            //    Margin = new Thickness(4)
            //};

            var grid = new Grid
            {
                Margin = new Thickness(4)
            };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var descriptionAttr = prop.GetCustomAttribute<DescriptorAttribute>();
            var labelText = descriptionAttr?.Description ?? "MISSING DESCRIPTION";

            var label = new TextBlock
            {
                Text = labelText,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 500,
                FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                FontSize = 6,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Regular,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.WrapWithOverflow,
            };

            Grid.SetColumn(label, 0);

            FrameworkElement inputControl;

            if (prop.PropertyType == typeof(bool))
            {
                //inputControl = new CheckBox
                //{
                //    IsChecked = value is bool b && b,
                //    VerticalAlignment = VerticalAlignment.Center
                //};
                inputControl = new ToggleSwitch
                {
                    IsOn = value is bool b && b,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100,
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    FontSize = 8,
                    OnContent = "Yes",
                    OffContent = "No"
                };
            }
            else if (prop.PropertyType == typeof(int))
            {
                var slider = new Slider
                {
                    Minimum = 0,
                    Maximum = 100,
                    Width = 150,
                    Value = value is int i ? i : 0,
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    FontSize = 8,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Optional: apply [Range] attribute
                var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
                if (rangeAttr != null)
                {
                    slider.Minimum = (int)rangeAttr.Minimum;
                    slider.Maximum = (int)rangeAttr.Maximum;
                }

                inputControl = slider;
            }
            else if (prop.PropertyType == typeof(string))
            {
                inputControl = new System.Windows.Controls.TextBox
                {
                    Text = value?.ToString() ?? "",
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    FontSize = 8,
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            else
            {
                // fallback label for unsupported types
                inputControl = new TextBlock
                {
                    Text = $"Unsupported: {prop.PropertyType.Name}",
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    FontSize = 6,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            inputControl.Tag = prop;
            Grid.SetColumn(inputControl, 1);

            grid.Children.Add(label);
            grid.Children.Add(inputControl);
            return grid;
        }

        public static ElementHost ElementHost(FrameworkElement CreateLabeledPanel) 
        {
            var button = new ElementHost
            {
                Child = CreateLabeledPanel,
                AutoSize = true,
                Margin = new Padding(1),
                Dock = DockStyle.Top
            };

            return button;
        }

        public static System.Windows.Forms.Control CreateVoiceSelectionControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };
  
            var label = new System.Windows.Forms.Label
            {
                Text = "Voice:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            panel.Controls.Add(label, 0, 0);

            var comboBox = new System.Windows.Forms.ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill
            };

            PopulateVoices(comboBox, preferences);

            panel.Controls.Add(comboBox, 1, 0);

            return panel;
        }

        public static System.Windows.Forms.Control CreateRateControl(PropertyInfo prop, IPreferences preferences, Action<IPreferences> onPreferenceModified = null)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            var label = new System.Windows.Forms.Label
            {
                Text = "Speaking Rate:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            panel.Controls.Add(label, 0, 0);

            var currentRate = (int)prop.GetValue(preferences);
            var rateRange = GetRateRange(preferences);

            var numericUpDown = new System.Windows.Forms.NumericUpDown
            {
                Minimum = rateRange.Min,
                Maximum = rateRange.Max,
                Value = currentRate,
                Increment = 1,
                Dock = DockStyle.Fill
            };

            numericUpDown.ValueChanged += (sender, e) =>
            {
                prop.SetValue(preferences, (int)numericUpDown.Value);
                onPreferenceModified?.Invoke(preferences);// TODO repeat on each numericUpDown
            };

            panel.Controls.Add(numericUpDown, 1, 0);

            var rangeLabel = new System.Windows.Forms.Label
            {
                Text = $"({rateRange.Min} to {rateRange.Max})",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = Color.Gray
            };
            panel.Controls.Add(rangeLabel, 2, 0);

            return panel;
        }

        public static System.Windows.Forms.Control CreateVolumeControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            // Add label  
            var label = new System.Windows.Forms.Label
            {
                Text = "Volume:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            panel.Controls.Add(label, 0, 0);

            // Get current volume value and range  
            var currentVolume = (int)prop.GetValue(preferences);
            var volumeRange = GetVolumeRange(preferences);

            // Create numeric up/down control for volume  
            var numericUpDown = new System.Windows.Forms.NumericUpDown
            {
                Minimum = volumeRange.Min,
                Maximum = volumeRange.Max,
                Value = currentVolume,
                Increment = 5, // Volume increments by 5  
                Dock = DockStyle.Fill
            };

            // Update preferences when value changes  
            numericUpDown.ValueChanged += (sender, e) =>
            {
                prop.SetValue(preferences, (int)numericUpDown.Value);
            };

            panel.Controls.Add(numericUpDown, 1, 0);

            // Add range label  
            var rangeLabel = new System.Windows.Forms.Label
            {
                Text = $"({volumeRange.Min} to {volumeRange.Max})",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = Color.Gray
            };
            panel.Controls.Add(rangeLabel, 2, 0);

            return panel;
        }

        public static System.Windows.Forms.Control CreatePitchControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            var label = new System.Windows.Forms.Label
            {
                Text = "Pitch:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            panel.Controls.Add(label, 0, 0);

            var currentPitch = (int)prop.GetValue(preferences);
            var pitchRange = GetPitchRange(preferences);

            var numericUpDown = new System.Windows.Forms.NumericUpDown
            {
                Minimum = pitchRange.Min,
                Maximum = pitchRange.Max,
                Value = currentPitch,
                Increment = 1,
                Dock = DockStyle.Fill
            };

            numericUpDown.ValueChanged += (sender, e) =>
            {
                prop.SetValue(preferences, (int)numericUpDown.Value);
            };

            panel.Controls.Add(numericUpDown, 1, 0);

            var rangeLabel = new System.Windows.Forms.Label
            {
                Text = $"({pitchRange.Min} to {pitchRange.Max})",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = Color.Gray
            };
            panel.Controls.Add(rangeLabel, 2, 0);

            return panel;
        }

        private static (int Min, int Max) GetVolumeRange(IPreferences preferences)
        {
            var engineType = DetermineEngineType(preferences);

            return engineType switch
            {
                "SAPI" => (0, 100),       
                "TTSClient" => (0, 100),   
                _ => (0, 100)           
            };
        }

        private static (int Min, int Max) GetRateRange(IPreferences preferences)
        {
            var engineType = DetermineEngineType(preferences);

            return engineType switch
            {
                "SAPI" => (-10, 10),      
                "TTSClient" => (-10, 10),    
                _ => (-10, 10)         
            };
        }

        private static (int Min, int Max) GetPitchRange(IPreferences preferences)
        {
            var engineType = DetermineEngineType(preferences);

            return engineType switch
            {
                "SAPI" => (int.MinValue, int.MaxValue),      // SAPI pitch range (not supported)  
                "TTSClient" => (int.MinValue, int.MaxValue), 
                _ => (-10, 10)                               
            };
        }

        private static void PopulateVoices(System.Windows.Forms.ComboBox comboBox, IPreferences preferences)
        {
            comboBox.Items.Clear();

            var engineType = DetermineEngineType(preferences);

            switch (engineType)
                {
                    case "SAPI":
                        try
                        {
                            using (var synthesizer = new SpeechSynthesizer())
                            {
                                var voices = synthesizer.GetInstalledVoices();
                                foreach (InstalledVoice voice in voices)
                                {
                                    comboBox.Items.Add(voice.VoiceInfo.Name);
                                }
                            }
                        }
                        catch
                        {
                            comboBox.Items.Add("Default SAPI Voice");
                        }
                        break;

                    case "TTSClient":
                        comboBox.Items.Add("Server Default");   // TTS Client doesn't support voice selection
                        comboBox.Enabled = false;               // Disable since no voice selection available 
                        break;

                    case "NullTTS":
                        comboBox.Items.Add("No Voice (Null Engine)");
                        comboBox.Enabled = false;
                        break;

                    default:
                        comboBox.Items.Add("Default Voice");
                        comboBox.Items.Add("System Default");
                        break;
                }
        }

        private static string DetermineEngineType(IPreferences preferences)
        {
            var descriptor = preferences.GetType().GetCustomAttribute<DescriptorAttribute>();

            if (descriptor != null)
            {
                switch (descriptor.Name)
                {
                    case "SAPI Engine":
                        return "SAPI";

                    case "TTS Client":
                        return "TTSClient";

                    case "Null TTS Engine":
                        return "NullTTS";

                    default:
                        return "Generic";
                }
            }

            return "Generic";
        }

        public static System.Windows.Forms.Control CreateWordCountControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            var label = new System.Windows.Forms.Label
            {
                Text = "Word Count:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            panel.Controls.Add(label, 0, 0);

            var currentValue = (int)prop.GetValue(preferences);
            var numericUpDown = new System.Windows.Forms.NumericUpDown
            {
                Minimum = 1,
                Maximum = 20,
                Value = currentValue,
                Increment = 1,
                Dock = DockStyle.Fill
            };

            numericUpDown.ValueChanged += (sender, e) =>
            {
                prop.SetValue(preferences, (int)numericUpDown.Value);
            };

            panel.Controls.Add(numericUpDown, 1, 0);

            var rangeLabel = new System.Windows.Forms.Label
            {
                Text = "(1 to 20)",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = Color.Gray
            };
            panel.Controls.Add(rangeLabel, 2, 0);

            return panel;
        }

        public static System.Windows.Forms.Control CreatePunctuationFilterControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            var checkBox = new System.Windows.Forms.CheckBox
            {
                Text = "Filter Punctuations",
                Checked = (bool)prop.GetValue(preferences),
                AutoSize = true
            };

            checkBox.CheckedChanged += (sender, e) =>
            {
                prop.SetValue(preferences, checkBox.Checked);
            };

            panel.Controls.Add(checkBox, 0, 0);

            return panel;
        }

        public static System.Windows.Forms.Control CreateNGramControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            var label = new System.Windows.Forms.Label
            {
                Text = "N-Gram Size:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            panel.Controls.Add(label, 0, 0);

            var currentValue = (int)prop.GetValue(preferences);
            var numericUpDown = new System.Windows.Forms.NumericUpDown
            {
                Minimum = 1,
                Maximum = 5,
                Value = currentValue,
                Increment = 1,
                Dock = DockStyle.Fill
            };

            numericUpDown.ValueChanged += (sender, e) =>
            {
                prop.SetValue(preferences, (int)numericUpDown.Value);
            };

            panel.Controls.Add(numericUpDown, 1, 0);

            var rangeLabel = new System.Windows.Forms.Label
            {
                Text = "(1 to 5)",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = Color.Gray
            };
            panel.Controls.Add(rangeLabel, 2, 0);

            return panel;
        }

        public static System.Windows.Forms.Control CreateLearningControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            var checkBox = new System.Windows.Forms.CheckBox
            {
                Text = "Enable Learning",
                Checked = (bool)prop.GetValue(preferences),
                AutoSize = true
            };

            checkBox.CheckedChanged += (sender, e) =>
            {
                prop.SetValue(preferences, checkBox.Checked);
            };

            panel.Controls.Add(checkBox, 0, 0);

            var descriptionLabel = new System.Windows.Forms.Label
            {
                Text = "Allow the word predictor to learn from user input",
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Montserrat", 10, FontStyle.Italic),
            };
            panel.Controls.Add(descriptionLabel, 1, 0);

            return panel;
        }
        public static System.Windows.Forms.Control CreateFilterCharsControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            var label = new System.Windows.Forms.Label
            {
                Text = "Filter Characters:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            panel.Controls.Add(label, 0, 0);

            var textBox = new System.Windows.Forms.TextBox
            {
                Text = (string)prop.GetValue(preferences) ?? string.Empty,
                Dock = DockStyle.Fill,
                Multiline = false
            };

            textBox.TextChanged += (sender, e) =>
            {
                prop.SetValue(preferences, textBox.Text);
            };

            panel.Controls.Add(textBox, 1, 0);

            var descriptionLabel = new System.Windows.Forms.Label
            {
                Text = "Characters to filter out from predicted words (e.g., punctuations)",
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Montserrat", 10, FontStyle.Italic),
            };
            panel.Controls.Add(descriptionLabel, 0, 1);
            panel.SetColumnSpan(descriptionLabel, 2);

            return panel;
        }

        public static System.Windows.Forms.Control CreateEncodingControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            var checkBox = new System.Windows.Forms.CheckBox
            {
                Text = "Use Default Encoding",
                Checked = (bool)prop.GetValue(preferences),
                AutoSize = true
            };

            checkBox.CheckedChanged += (sender, e) =>
            {
                prop.SetValue(preferences, checkBox.Checked);
            };

            panel.Controls.Add(checkBox, 0, 0);

            var descriptionLabel = new System.Windows.Forms.Label
            {
                Text = "Enable if the ConvAssist database requires encoding translation",
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Montserrat", 10, FontStyle.Italic),
            };
            panel.Controls.Add(descriptionLabel, 0, 1);

            return panel;
        }

        public static System.Windows.Forms.Control CreateDisclaimerControl(PropertyInfo prop, IPreferences preferences)
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            var checkBox = new System.Windows.Forms.CheckBox
            {
                Text = "Show Disclaimer on Startup",
                Checked = (bool)prop.GetValue(preferences),
                AutoSize = true
            };

            checkBox.CheckedChanged += (sender, e) =>
            {
                prop.SetValue(preferences, checkBox.Checked);
            };

            panel.Controls.Add(checkBox, 0, 0);

            var descriptionLabel = new System.Windows.Forms.Label
            {
                Text = "Display disclaimer dialog when the application starts",
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Montserrat", 10, FontStyle.Italic),
            };
            panel.Controls.Add(descriptionLabel, 0, 1);

            return panel;
        }

    }
}
