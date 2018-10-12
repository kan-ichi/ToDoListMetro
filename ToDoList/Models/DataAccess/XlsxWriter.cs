using ClosedXML.Excel;
using System;
using System.Collections.Generic;
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
            var sheet = book.Worksheets.Add("ToDoListMetro_Export");

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

            book.SaveAs(_fileName);
        }

        /// <summary>
        /// FileView ボタン〔バックアップ〕のデータ出力を行います
        /// </summary>
        public static void FileViewBackup(string _fileName, Dictionary<string, List<List<object>>> _tables)
        {
            var book = new XLWorkbook();

            foreach (var table in _tables)
            {
                var sheet = book.Worksheets.Add(table.Key);
                int rowNum = 1;

                List<List<object>> records = table.Value;
                foreach (List<object> record in records)
                {
                    int colNum = 1;

                    foreach (var column in record)
                    {
                        sheet.Cell(rowNum, colNum).SetValue<string>(column.ToString());
                        colNum++;
                    }

                    rowNum++;
                }
            }

            book.SaveAs(_fileName);
        }
    }
}
