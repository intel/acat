using ACAT.Core.Utility;
using ACATResources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.openBCISensorUI
{
    internal class Utils
    {
        private static readonly ILogger<Utils> _logger = LoggingConfiguration.CreateLogger<Utils>();

        internal static void HandleHelpNavigation(WebBrowserNavigatingEventArgs e)
        {
            var str = e.Url.ToString();

            _logger.LogDebug("Url is [" + str + "]");

            if (str.ToLower().Contains("blank"))
            {
                return;
            }

            e.Cancel = true;

            String param1 = String.Empty;
            String param2 = String.Empty;

            if (str.Contains("about:"))
            {
                var index = str.IndexOf(':');

                str = str.Substring(index + 1);

                index = str.IndexOf('#');

                if (index > 0)
                {
                    param1 = str.Substring(0, index);
                    param2 = str.Substring(index + 1, str.Length - index - 1);
                }
                else
                {
                    param1 = str;
                }
            }

            List<String> list = new();

            if (param2.ToLower().EndsWith(".mp4"))
            {
                list.Add("Video");
                list.Add(String.Empty);
                list.Add(String.Empty);
                list.Add((param2));
                list.Add(String.Empty);
            }
            else if (param1.ToLower().EndsWith(".pdf"))
            {
                list.Add("PDF");
                list.Add("true");
                list.Add(StringResources.PDFLoaderHtml);
                list.Add(param1);
                list.Add(param2);
            }

            try
            {
                HtmlUtils.LoadHtml(SmartPath.ApplicationPath, list.ToArray());
            }
            catch
            {
            }
            finally
            {
            }
        }
    }
}