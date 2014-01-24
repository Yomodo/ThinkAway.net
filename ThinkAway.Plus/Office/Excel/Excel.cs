using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ThinkAway.Plus.Office.Excel.Styles;

namespace ThinkAway.Plus.Office.Excel
{
    public class Excel
    {
        /// <summary>
        /// 描述一个 Workbook 对象集合
        /// </summary>
        public WorkbookCollection Workbooks { get; set; }

        public Excel()
        {
            Workbooks = new WorkbookCollection();
        }

        public Workbook Open(string fileName)
        {
            TextReader textReader = new StreamReader(fileName);
            XmlReader xmlReader = XmlReader.Create(textReader);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Workbook));
            object deserialize = xmlSerializer.Deserialize(xmlReader, new XmlDeserializationEvents());
            return (Workbook) deserialize;
        }

        public void ExportExcel(DataSet dataSet)
        {
            Excel excel = new Excel();
            Workbook workbook = excel.Workbooks.Add();

            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                DataTable dataTable = dataSet.Tables[i];
                Worksheet worksheet = new Worksheet(dataTable.TableName);
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    worksheet[1, j + 1] = new Cell(dataTable.Columns[j].ColumnName);
                }
                for (int k = 0; k < dataTable.Rows.Count; k++)
                {
                    for (int l = 0; l < dataTable.Columns.Count; l++)
                    {
                        worksheet[k + 2, l + 1] = new Cell(dataTable.Rows[k][l]);
                    }
                }

                worksheet.Tables.Rows[1].Height = 100;
                worksheet.Tables.Columns[2].Width = 200;
                worksheet[1, 2].StyleId = workbook.Styles.Add(new Style
                                                                  {
                                                                      Font = new Font
                                                                                 {
                                                                                     Color = "red",
                                                                                     Size = 16
                                                                                 }
                                                                      ,
                                                                      Alignment = new Alignment { Horizontal = "Center" }
                                                                  });
                worksheet[1, 1].StyleId = workbook.Styles.Add(new Style
                                                                  {
                                                                      Font = new Font
                                                                                 {
                                                                                     Color = "red",
                                                                                     Size = 16
                                                                                 }
                                                                      ,
                                                                      Alignment = new Alignment { Horizontal = "Center" }
                                                                  });

                workbook.WorkSheets.Add(worksheet);
                workbook.Save(dataTable.TableName + ".xml");
            }
        }

    }
    public class WorkbookCollection :List<Workbook>
    {
        public Workbook Add()
        {
            Workbook workbook = new Workbook();
            this.Add(workbook);
            return workbook;
        }
    }
}
