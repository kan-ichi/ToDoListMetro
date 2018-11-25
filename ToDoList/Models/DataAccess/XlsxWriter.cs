using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Entities;

namespace ToDoList.Models.DataAccess
{
    class XlsxWriter
    {
        /// <summary>
        /// FileView ボタン〔エクスポート〕のデータ出力を行います
        /// </summary>
        public static void FileViewExport(string _fileName, List<TodoTask> _tasks)
        {
            var book = new XLWorkbook();
            var sheet = book.Worksheets.Add("ToDoListMetro");

            int rowNum = 1;
            const int COL_NUM_DUE_DATE = 1;
            const int COL_NUM_STATUS_NAME = 2;
            const int COL_NUM_SUBJECT = 3;

            {
                sheet.Cell(rowNum, COL_NUM_DUE_DATE).Value = "期日";
                sheet.Cell(rowNum, COL_NUM_STATUS_NAME).Value = "状況";
                sheet.Cell(rowNum, COL_NUM_SUBJECT).Value = "内容";
                rowNum++;
            }

            foreach(var task in _tasks)
            {
                sheet.Cell(rowNum, COL_NUM_DUE_DATE).Style.DateFormat.Format = "yyyy/mm/dd hh:mm";
                sheet.Cell(rowNum, COL_NUM_DUE_DATE).Value = task.DueDate;
                sheet.Cell(rowNum, COL_NUM_STATUS_NAME).Value = task.StatusCode.Name;
                sheet.Cell(rowNum, COL_NUM_SUBJECT).Value = task.Subject;
                rowNum++;
            }

            book.Protect();
            book.SaveAs(_fileName);
        }

        /// <summary>
        /// FileView ボタン〔バックアップ〕のデータ出力を行います
        /// </summary>
        public static void FileViewBackup(string _fileName, DataSet _tables)
        {
            var book = new XLWorkbook();

            foreach (DataTable table in _tables.Tables)
            {
                var sheet = book.Worksheets.Add(table.TableName);
                int rowNum = 1;

                {
                    int colNum = 1;
                    foreach (DataColumn column in table.Columns)
                    {
                        sheet.Cell(rowNum, colNum).SetValue<string>(column.ColumnName);
                        colNum++;
                    }

                    rowNum++;
                }

                foreach (DataRow record in table.Rows)
                {
                    int colNum = 1;
                    foreach (DataColumn column in table.Columns)
                    {
                        sheet.Cell(rowNum, colNum).SetValue<string>(Convert.ToString(record[column.ColumnName]));
                        colNum++;
                    }

                    rowNum++;
                }

                sheet.Protect();
            }

            book.Protect();
            book.SaveAs(_fileName);
        }
    }
}
