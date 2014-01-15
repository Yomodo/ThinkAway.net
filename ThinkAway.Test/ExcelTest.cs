using ThinkAway.Plus.Office.Excel;
using ThinkAway.Plus.Office.Excel.Styles;

namespace ThinkAway.Test
{
    class ExcelTest
    {
        private readonly Excel _excel;

        public ExcelTest()
        {
            _excel = new Excel();
        }

        public void Test()
        {
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet("Test");
            string styleId = workbook.Styles.Add(new Style
                                                     {
                                                         Alignment = new Alignment
                                                                         {
                                                                             Horizontal = "Center"
                                                                         }

                                                     });
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j <= i; j++)
                {
                    string str = string.Format("{0} X {1} = {2}", j, i, i*j);
                    worksheet.Tables.Columns[i].Width = 65;
                    worksheet.Tables.Rows[i].Cells[j] = new Cell(str)
                                                            {
                                                                StyleId = styleId
                                                            };
                }
            }
            workbook.WorkSheets.Add(worksheet);
            _excel.Workbooks.Add(workbook);
            workbook.Save("c:\\aa.xml");
        }
    }
}
