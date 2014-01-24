using System.Data;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace ThinkAway.Plus.Office
{
    public class OfficeHelper
    {
        private readonly Application _excelApplication;

        public OfficeHelper()
        {
            //create excel application instance .
            _excelApplication = new ApplicationClass();
        }

        /// <summary>
        /// 根据指定的 DataSet 数据集,导出Excel文件
        /// <c>ExportExcel</c>
        /// </summary>
        /// <param name="dataSet">DataSet</param>
        public void ExportExcel(DataSet dataSet)
        {
            int rowOffset = 1;
            int columnOffset = 1;

            //get tables in DataSet object .
            foreach (DataTable dataTable in dataSet.Tables)
            {
                //create workbook in excel .
                Workbook workbook = _excelApplication.Workbooks.Add();

                //draw excel header text .
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    _excelApplication.Cells[rowOffset, i + columnOffset] = dataTable.Columns[i].ColumnName;
                }

                //next of offset .
                rowOffset++;

                //draw row .
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        _excelApplication.Cells[i + rowOffset, j + columnOffset] = dataTable.Rows[i][j];
                    }
                }

                ////save to excel file .
                workbook.SaveAs("C:\\test.xlsx");
                ////close workbook object .
                //workbook.Close();
            }  
            //excelApplication.DefaultSaveFormat = XlFileFormat.xlExcel9795;
            //excelApplication.Save("Sheet2");
            //quit application .
            _excelApplication.Quit();
        }
    }
}
