using System.Data;
using ThinkAway.Plus.Office.Excel;

namespace ThinkAway.Test
{
    class OfficeHelper
    {
        public void ExportExcel(DataSet dataSet)
        {
            Excel excel = new Excel();
            Workbook workbook = excel.Workbooks.Add();

            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                DataTable dataTable = dataSet.Tables[i];
                Worksheet worksheet = new Worksheet();
                worksheet.Name = dataTable.TableName;
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
                worksheet.Tables.Columns[1].Width = 100;
                //workSheet[1, 2].StyleId = workbook.Styles.Add(new Style
                //                                                  {
                //                                                      Font = new Font
                //                                                                 {
                //                                                                     Color = "red",
                //                                                                     Size = 16
                //                                                                 }
                //                                                      ,
                //                                                      Alignment = new Alignment {Horizontal = "Center"}
                //                                                  });
                //workSheet[1, 1].StyleId = workbook.Styles.Add(new Style
                //                                                  {
                //                                                      Font = new Font
                //                                                                 {
                //                                                                     Color = "red",
                //                                                                     Size = 16
                //                                                                 }
                //                                                      ,
                //                                                      Alignment = new Alignment {Horizontal = "Center"}
                //                                                  });
                //workbook.WorkSheets.Add(worksheet);

                //workbook.Save("file.xml");
                //Excel excel = new Excel();
                //Workbook workbook = excel.Open("file.xml");



                workbook.WorkSheets.Add(worksheet);
                workbook.Save(dataTable.TableName + ".xml");
            }
        }
    }
}
