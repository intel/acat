using ACAT.ACATResources;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    internal class Utils
    {
        internal static void HandleHelpNavigaion(WebBrowserNavigatingEventArgs e)
        {
            var str = e.Url.ToString();

            Log.Debug("Url is [" + str + "]");

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

            List<String> list = new List<String>();

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
                list.Add(R.GetString("PDFLoaderHtml"));
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