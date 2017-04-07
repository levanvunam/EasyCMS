using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ez.Framework.Utilities.Html
{
    /// <summary>
    /// HTML utilities
    /// </summary>
    public static class HtmlUtilities
    {
        /// <summary>
        /// Remove all element in html that has specific classes
        /// </summary>
        /// <param name="html"></param>
        /// <param name="classNames"></param>
        /// <returns></returns>
        public static string RemoveElementsHasClass(this string html, params string[] classNames)
        {
            if (string.IsNullOrEmpty(html) || classNames == null || !classNames.Any())
            {
                return html;
            }

            HtmlNode.ElementsFlags.Remove("option");
            var document = new HtmlDocument();
            document.LoadHtml(html);

            if (document.ParseErrors != null && document.ParseErrors.Any())
            {
                return html;
            }

            int maxLoop = 100;
            if (document.DocumentNode != null)
            {
                maxLoop--;
                HtmlNode removedNode;
                do
                {
                    removedNode = document.DocumentNode.Descendants().FirstOrDefault(d => d.Attributes.Contains("class") &&
                                    classNames.Any(c => d.Attributes["class"].Value.Contains(c)));

                    if (removedNode != null)
                    {
                        removedNode.Remove();
                    }

                } while (removedNode != null && maxLoop > 0);

                return document.DocumentNode.OuterHtml;
            }
            return html;
        }

        /// <summary>
        /// Build select html
        /// </summary>
        /// <param name="items"></param>
        /// <param name="optionLabel"></param>
        /// <returns></returns>
        public static string BuildSelectHtml(this IEnumerable<SelectListItem> items, string optionLabel = "")
        {
            var tag = new TagBuilder("select");
            tag.MergeAttribute("class", "form-control");

            tag.InnerHtml = BuildOptionsHtml(items, optionLabel);

            return tag.ToString();
        }

        /// <summary>
        /// Build options html
        /// </summary>
        /// <param name="items"></param>
        /// <param name="optionLabel"></param>
        /// <returns></returns>
        public static string BuildOptionsHtml(this IEnumerable<SelectListItem> items, string optionLabel = "")
        {
            var html = string.Empty;

            // Append default label
            if (!string.IsNullOrEmpty(optionLabel))
            {
                var option = new TagBuilder("option");
                option.MergeAttribute("value", string.Empty);
                option.InnerHtml = optionLabel;

                html += option.ToString();
            }

            foreach (var item in items)
            {
                var option = new TagBuilder("option");
                option.MergeAttribute("value", item.Value);
                option.InnerHtml = item.Text;

                if (item.Selected)
                {
                    option.MergeAttribute("selected", "selected");
                }

                html += option.ToString();
            }

            return html;
        }

        /// <summary>
        /// Build script
        /// </summary>
        /// <param name="action"></param>
        /// <param name="data"></param>
        /// <param name="needWrapper"></param>
        /// <returns></returns>
        public static string BuildScript(ScriptAction action, string data, bool needWrapper = true)
        {
            var script = string.Empty;
            switch (action)
            {
                case ScriptAction.Alert:
                    script = string.Format("alert('{0}')", data);
                    break;
                case ScriptAction.Redirect:
                    script = string.Format("window.location.href = '{0}'", data);
                    break;
                case ScriptAction.Write:
                    script = string.Format("document.write('{0}')", data);
                    break;
            }

            if (needWrapper)
            {
                script = string.Format("<script>{0}</script>", script);
            }

            return script;
        }

        /// <summary>
        /// Convert environment new line to html for mat
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Nl2Br(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var builder = new StringBuilder();
            var lines = text.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                    builder.Append("<br/>");
                builder.Append(HttpUtility.HtmlEncode(lines[i]));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Remove environment new line from text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string RemoveNewLine(this string text, string replace = "")
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            text = text.Replace(Environment.NewLine, replace);
            text = text.Replace("\n", replace);

            return text;
        }

        /// <summary>
        /// Check a html string content an url
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool CheckContentUrl(this string text, string url)
        {
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(url))
            {
                //Get available url that can be matched
                var urls = new List<string>
                {
                    url,
                    url.ToPageFriendlyUrl(),
                    url.ToAbsoluteUrl()
                };

                //Load current html to document
                var doc = new HtmlDocument();
                doc.LoadHtml(text);

                //Get available links
                var links = doc.DocumentNode.SelectNodes("//a/@href");
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        var attribute = link.Attributes["href"];
                        if (urls.Contains(attribute.Value))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Replace url in a html string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="oldUrl"></param>
        /// <param name="newUrl"></param>
        /// <returns></returns>
        public static string ReplaceUrl(this string text, string oldUrl, string newUrl)
        {
            //If text is empty then return text
            if (string.IsNullOrEmpty(text))
                return text;

            //Load current text to html document
            var doc = new HtmlDocument();
            doc.LoadHtml(text);

            //Get all available anchor in document
            var links = doc.DocumentNode.SelectNodes("//a/@href");

            foreach (var link in links)
            {
                var att = link.Attributes["href"];
                if (att.Value.Equals(oldUrl))
                {
                    att.Value = newUrl;
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Convert html to text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ConvertHtmlToText(this string html)
        {
            return HtmlToText.ConvertHtml(html);
        }
    }

    public enum ScriptAction
    {
        Alert = 1,
        Redirect,
        Write
    }
}
