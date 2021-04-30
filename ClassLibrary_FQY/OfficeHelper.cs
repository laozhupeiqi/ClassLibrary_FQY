using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassLibrary_FQY
{
    /// <summary>
    /// Office使用类
    /// 需要引用microsoft.office.interop.excel
    /// 
    /// </summary>
    public class OfficeHelper
    {
        /// <summary>
        /// 将dataGridView导出为Excel
        /// </summary>
        /// <param name="myDGV">dataGridView名称</param>
        /// <param name="filename">导出路径</param>
        public static void ExportExcel(DataGridView myDGV, string filename)
        {
            string saveFilename = filename;
            if (saveFilename==null || saveFilename.IndexOf(":") < 0)
            {
                return;
            }

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象");
                return;
            }

            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1

            //写入标题
            for (int i = 0; i < myDGV.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = myDGV.Columns[i].HeaderText;
            }

            //写入数值
            for (int r = 0; r < myDGV.Rows.Count; r++)
            {
                for (int i = 0; i < myDGV.ColumnCount; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = myDGV.Rows[r].Cells[i].Value;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应


            if (saveFilename != null)
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFilename);
                    MessageBox.Show("文件 " + filename + "保存成功", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出文件时出错，文件可能正被打开！\n" + ex.Message);
                }
            }
            xlApp.Quit();
            GC.Collect();
        }

    }
}
