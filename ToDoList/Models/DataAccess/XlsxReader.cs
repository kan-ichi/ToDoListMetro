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
    class XlsxReader
    {
        /// <summary>
        /// xlsx ファイルのデータを取得します
        /// </summary>
        public static DataSet GetXLSheets(string _fileName)
        {
            List<string> sheetNames = GetSheetNames(_fileName);
            DataSet sheets = new DataSet();

            using (var xlBook = new XLWorkbook(_fileName))
            {
                foreach(var sheetName in sheetNames)
                {
                    IXLWorksheet xlSheet = xlBook.Worksheet(sheetName);
                    DataTable rowList = GetXLSheet(xlSheet);
                    sheets.Tables.Add(rowList);
                }
            }

            return sheets;
        }

        /// <summary>
        /// xlsx ファイルのシート名一覧を取得します
        /// </summary>
        private static List<string> GetSheetNames(string _fileName)
        {
            List<string> sheetNames = new List<string>();

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
        private static DataTable GetXLSheet(IXLWorksheet _xlSheet)
        {
            DataTable rowList = new DataTable(_xlSheet.Name);

            IXLTable xlTable = _xlSheet.RangeUsed().AsTable();
            int maxRowIndex = xlTable.DataRange.RowCount();
            int maxColIndex = xlTable.DataRange.ColumnCount();

            for (int colIndex = 1; colIndex <= maxColIndex; colIndex++)
            {
                rowList.Columns.Add(new DataColumn());
            }

            for (int rowIndex = 1; rowIndex <= maxRowIndex + 1; rowIndex++)
            {
                var row = rowList.NewRow();
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
                    row[colIndex - 1] = columnValue;
                }
                rowList.Rows.Add(row);
            }

            return rowList;
        }
    }
}
