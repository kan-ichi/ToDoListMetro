using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Entities;

namespace ToDoList.Models.DataAccess
{
    class XlsxReader
    {
        /// <summary>
        /// xlsx ファイルのデータを取得します
        /// </summary>
        public static Dictionary<string, List<List<object>>> GetXLSheets(string _fileName)
        {
            List<string> sheetNames = GetSheetNames(_fileName);
            var sheets = new Dictionary<string, List<List<object>>>();

            using (var xlBook = new XLWorkbook(_fileName))
            {
                foreach(var sheetName in sheetNames)
                {
                    IXLWorksheet xlSheet = xlBook.Worksheet(sheetName);
                    List<List<object>> rowList = GetXLSheet(xlSheet);
                    sheets.Add(sheetName, rowList);
                }
            }

            return sheets;
        }

        /// <summary>
        /// xlsx ファイルのシート名一覧を取得します
        /// </summary>
        private static List<string> GetSheetNames(string _fileName)
        {
            var sheetNames = new List<string>();

            using (var xlBook = new XLWorkbook(_fileName))
            {
                foreach(IXLWorksheet xlSheet in xlBook.Worksheets)
                {
                    sheetNames.Add(xlSheet.Name);
                }
            }

            return sheetNames;
        }

        /// <summary>
        /// xlsx シートのデータを取得します
        /// </summary>
        private static List<List<object>> GetXLSheet(IXLWorksheet _xlSheet)
        {
            var rowList = new List<List<object>>();

            IXLTable xlTable = _xlSheet.RangeUsed().AsTable();
            int maxRowIndex = xlTable.DataRange.RowCount();
            int maxColIndex = xlTable.DataRange.ColumnCount();

            for (int rowIndex = 1; rowIndex <= maxRowIndex; rowIndex++)
            {
                var row = new List<object>();
                for (int colIndex = 1; colIndex <= maxColIndex; colIndex++)
                {
                    IXLCell xlCell = _xlSheet.Cell(rowIndex, colIndex);
                    string columnValue = string.Empty;
                    switch (xlCell.DataType)
                    {
                        case XLDataType.Number:
                            columnValue = _xlSheet.Cell(rowIndex, colIndex).GetFormattedString();
                            break;

                        case XLDataType.DateTime:
                            DateTime dt = _xlSheet.Cell(rowIndex, colIndex).GetDateTime();
                            columnValue = dt.ToString();
                            break;

                        case XLDataType.Text:
                            columnValue = _xlSheet.Cell(rowIndex, colIndex).GetString();
                            break;
                    }
                    row.Add(columnValue);
                }
                rowList.Add(row);
            }

            return rowList;
        }
    }
}
