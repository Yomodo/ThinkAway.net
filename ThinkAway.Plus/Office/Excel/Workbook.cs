using System;
using System.Xml;
using System.Xml.Serialization;
using ThinkAway.Plus.Office.Excel.Misc;
using ThinkAway.Plus.Office.Excel.Styles;

namespace ThinkAway.Plus.Office.Excel
{
    /// <summary>
    /// Excel 描述一个 Workbook 对象
    /// </summary>
    [Serializable, XmlRoot("Workbook", Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class Workbook
    {
        /// <summary>
        /// DocumentProperties
        /// </summary>
        [XmlElementAttribute(ElementName = "DocumentProperties", Namespace = "http://schemas.lsong.org/office/excel")]
        public DocumentProperties Properties { get; set; }
        /// <summary>
        /// OfficeDocumentSettings
        /// </summary>
        [XmlElementAttribute(ElementName = "OfficeDocumentSettings")]
        public DocumentSettings DocumentSettings { get; set; }
        /// <summary>
        /// ExcelWorkbook
        /// </summary>
        [XmlElementAttribute(ElementName = "ExcelWorkbook")]
        public ExcelWorkbook ExcelWorkbook { get; set; }
        /// <summary>
        /// Styles
        /// </summary>
        [XmlElement(ElementName = "Styles")]
        public StyleCollection Styles { get; set; }
        /// <summary>
        /// 描述一个 Worksheet 对象集合
        /// </summary>
        [XmlElementAttribute("Worksheet")]
        public WorksheetCollection WorkSheets { get; set; }
        /// <summary>
        /// Workbook
        /// </summary>
        public Workbook()
        {
            Styles = new StyleCollection();

            Properties = new DocumentProperties();

            DocumentSettings = new DocumentSettings();

            ExcelWorkbook = new ExcelWorkbook();

            WorkSheets = new WorksheetCollection();
        }
        /// <summary>
        /// 将 Excel 工作簿 以XML文档方式 保存到指定的文件中
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("ss", "http://schemas.lsong.org/office");
            namespaces.Add("o", "urn:schemas-microsoft-com:office:office");
            namespaces.Add("x", "urn:schemas-microsoft-com:office:excel");
            namespaces.Add("html", "http://www.w3.org/TR/REC-html40");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.OmitXmlDeclaration = false;
            using (XmlWriter xmlWriter = XmlWriter.Create(fileName, settings))
            {
                xmlWriter.WriteProcessingInstruction("mso-application", "progid=\"Excel.Sheet\"");
                XmlSerializer xmlSerializer = new XmlSerializer(GetType());
                xmlSerializer.Serialize(xmlWriter, this, namespaces);
                xmlWriter.Flush();
                xmlWriter.Close();
            }
        }
    }
}
