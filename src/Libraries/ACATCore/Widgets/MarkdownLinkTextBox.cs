using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class MarkdownLinkTextBox : UserControl
{
    private FlowLayoutPanel _flowPanel;

    public MarkdownLinkTextBox()
    {
        // Initialize the FlowLayoutPanel
        _flowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51))))),
            ForeColor = Color.White
        };
        this.Controls.Add(_flowPanel);
    }

    private string _markdownText = "";
    /// <summary>
    /// The Markdown-formatted text. Any “[Text](URL)” fragments will become LinkLabels.
    /// Setting this property causes the control to rebuild its children.
    /// </summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Description("Markdown text with [LinkText](URL) segments. Plain text is shown as Label; links become clickable LinkLabel.")]
    public string MarkdownText
    {
        get => _markdownText;
        set
        {
            _markdownText = value ?? "";
            RebuildContent();
        }
    }

    /// <summary>
    /// Occurs when any of the generated LinkLabels is clicked.
    /// The UrlClicked event args contain the URL string.
    /// </summary>
    public event EventHandler<UrlClickedEventArgs> UrlClicked;

    /// <summary>
    /// Parses out “[Text](URL)” segments and repopulates the FlowLayoutPanel
    /// with Labels (for plain text) and LinkLabels (for hyperlinks).
    /// </summary>
    private void RebuildContent()
    {
        _flowPanel.Controls.Clear();

        // Regex to find [LinkText](URL)
        var pattern = new Regex(@"\[(.*?)\]\((.*?)\)", RegexOptions.Compiled);
        var matches = pattern.Matches(_markdownText);

        int lastIndex = 0;

        foreach (Match match in matches)
        {
            // 1) Add any plain text before this match
            if (match.Index > lastIndex)
            {
                string plain = _markdownText.Substring(lastIndex, match.Index - lastIndex);
                AddPlainLabel(plain);
            }

            // 2) Add the LinkLabel for this match
            string linkText = match.Groups[1].Value;
            string url = match.Groups[2].Value;
            AddLinkLabel(linkText, url);

            lastIndex = match.Index + match.Length;
        }

        // 3) If there's any trailing plain text after last match
        if (lastIndex < _markdownText.Length)
        {
            string trailing = _markdownText.Substring(lastIndex);
            AddPlainLabel(trailing);
        }
    }

    /// <summary>
    /// Helper: create a Label for plain text and add it to the panel.
    /// </summary>
    private void AddPlainLabel(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var lbl = new Label
        {
            AutoSize = true,
            Text = text,
            Margin = new Padding(0),
            TextAlign = ContentAlignment.MiddleLeft,
        };
        _flowPanel.Controls.Add(lbl);
    }

    /// <summary>
    /// Helper: create a LinkLabel for “linkText” pointing to “url”,
    /// subscribe to its click, and add to the panel.
    /// </summary>
    private void AddLinkLabel(string linkText, string url)
    {
        var lnk = new LinkLabel
        {
            AutoSize = true,
            Text = linkText,
            LinkColor = Color.Blue,
            ActiveLinkColor = Color.Red,
            Margin = new Padding(0),
            Tag = url, // store the URL in Tag
        };
        lnk.LinkClicked += (s, e) =>
        {
            string clickedUrl = (s as LinkLabel)?.Tag as string;
            // Raise our own UrlClicked event
            UrlClicked?.Invoke(this, new UrlClickedEventArgs(clickedUrl));

            // By default, attempt to open in browser:
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = clickedUrl,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch
            {
                // swallow or log if needed
            }
        };

        _flowPanel.Controls.Add(lnk);
    }
}

/// <summary>
/// EventArgs for when a link is clicked. Provides the raw URL string.
/// </summary>
public class UrlClickedEventArgs : EventArgs
{
    public string Url { get; }
    public UrlClickedEventArgs(string url) => Url = url;
}
