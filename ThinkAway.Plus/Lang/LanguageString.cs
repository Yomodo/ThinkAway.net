using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace ThinkAway.Plus.Lang
{
    /// <summary>
    /// 多语言字符串
    /// </summary>
    public class LanguageString
    {
        private static readonly Regex Pattern = new Regex(@"\$\{[^\}]+\}",
                                                          RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex PatternInner = new Regex(@"(?<=\${)([^\}]*)?(?=\})",
                                                               RegexOptions.Compiled | RegexOptions.CultureInvariant);


        /// <summary>
        /// 表达式获取当前语言对应的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetString(string input)
        {
            string result = Parse(input);
            string langFile = GetResurceFile();
            if (!File.Exists(langFile)) return input;
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(langFile);
            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Language/add");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                if (Equals(xmlNode.Attributes["name"].Value, result))
                {
                    result = xmlNode.InnerText;
                    break;
                }
            }
            return result;
        }

        public static string GetString(string input, string language)
        {
            string result = input;
            string langFile = GetResurceFile(language);
            if (!File.Exists(langFile)) return input;
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(langFile);
            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Language/add");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                if (Equals(xmlNode.Attributes["name"].Value, input))
                {
                    result = xmlNode.InnerText;
                    break;
                }
            }
            return result;
        }

        public static string Parse(string input)
        {
            string result = input;

            MatchCollection matchs = Pattern.Matches(input);
            foreach (Match match in matchs)
            {
                string resString = PatternInner.Match(match.Value).Value;
                resString = resString.Replace("$NewLine$", Environment.NewLine);
                result = result.Replace(match.Value, resString);
            }
            return result;
        }

        public static string GetResurceFile()
        {
            string fileName = "";
            var xmlDocument = new XmlDocument();
            xmlDocument.Load("app.config");
            XmlNode configXmlDocument = xmlDocument.SelectSingleNode("configuration/Language");
            if (configXmlDocument != null)
            {
                string language = configXmlDocument.Attributes["default"].Value;
                foreach (XmlNode xmlNode in configXmlDocument)
                {
                    if (Equals(xmlNode.Attributes["name"].Value, language))
                    {
                        fileName = xmlNode.InnerText;
                        break;
                    }
                }
            }
            return fileName;
        }

        public static string GetResurceFile(string language)
        {
            string fileName = "language.xml";
            var xmlDocument = new XmlDocument();
            xmlDocument.Load("app.config");
            const string xpath = "configuration/Language";
            XmlNode configXmlDocument = xmlDocument.SelectSingleNode(xpath);

            if (configXmlDocument != null)
            {
                foreach (XmlNode xmlNode in configXmlDocument)
                {
                    if (Equals(xmlNode.Attributes["name"].Value, language))
                    {
                        fileName = xmlNode.InnerText;
                        break;
                    }
                }
            }
            return fileName;
        }

        #region ApplyResures

        /// <summary>
        /// 应用所有资源
        /// </summary>
        /// <param name="userControl"></param>
        public static void ApplyResource(UserControl userControl)
        {
            userControl.Text = GetString(userControl.Text);

            foreach (Control control in userControl.Controls)
            {
                ApplyResource(control);
            }
        }

        public static void ApplyResource(Form form)
        {
            form.Text = GetString(form.Text);

            foreach (Control control in form.Controls)
            {
                ApplyResource(control);
            }
        }

        public static void ApplyResource(Control control)
        {
            control.Text = GetString(control.Text);
            foreach (Control ctrl in control.Controls)
            {
                ApplyResource(ctrl);
            }

            if (control.ContextMenuStrip != null)
            {
                ContextMenuStrip contextMenuStrip = control.ContextMenuStrip;
                ApplyResource(contextMenuStrip);
            }

            if (control is DataGridView)
            {
                var dataGridView = control as DataGridView;
                ApplyResource(dataGridView);
            }

            if (control is ToolStrip)
            {
                var toolStrip = control as ToolStrip;
                ApplyResource(toolStrip);
            }
        }

        public static void ApplyResource(DataGridView dataGridView)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderText = GetString(column.HeaderText);
            }
        }

        public static void ApplyResource(ToolStrip toolStrip)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                item.Text = GetString(item.Text);
                if (item is ToolStripDropDownItem)
                {
                    var toolStripDropDownItem = item as ToolStripDropDownItem;
                    ApplyResource(toolStripDropDownItem.DropDownItems);
                }
            }
        }

        public static void ApplyResource(ContextMenuStrip contextMenuStrip)
        {
            ApplyResource(contextMenuStrip.Items);
        }

        public static void ApplyResource(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.Text = GetString(item.Text);
                if (item is ToolStripMenuItem)
                {
                    var toolStripMenuItem = item as ToolStripMenuItem;
                    ApplyResource(toolStripMenuItem.DropDownItems);
                }
            }
        }

        /// <summary>
        /// 更新所有界面
        /// </summary>
        public static void ApplyResource()
        {
            foreach (Form form in Application.OpenForms)
            {
                ApplyResource(form);
            }
        }

        #endregion
    }
}