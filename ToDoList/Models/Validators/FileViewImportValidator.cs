using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Codes;
using ToDoList.Models.Utilities;

namespace ToDoList.Models.Validators
{
    /// <summary>
    /// FileView インポートのデータ検証を行います
    /// </summary>
    static class FileViewImportValidator
    {
        /// <summary>
        /// インポートデータの検証結果
        /// </summary>
        public enum CheckImportSheetsResult
        {
            /// <summary>
            /// シート名が不正
            /// </summary>
            SHEET_NAME_INVALID,

            /// <summary>
            /// ToDoListMetro シートに行が存在しない
            /// </summary>
            SHEET_TODO_TASK_ROW_NOT_FOUND,

            /// <summary>
            /// ToDoListMetro シートの列に過不足がある
            /// </summary>
            SHEET_TODO_TASK_COLUMN_INVALID,

            /// <summary>
            /// ［期日］の値が日付時刻形式で入力されていない
            /// </summary>
            COLUMN_DUE_DATE_INVALID_DATETIME,

            /// <summary>
            /// ［状況］の値がコード名称と一致していない
            /// </summary>
            COLUMN_STATUS_NAME_INVALID_CODE_NAME,

            /// <summary>
            /// ［状況］の値がブランク
            /// </summary>
            COLUMN_SUBJECT_BLANK
        }

        // インポートデータに存在するべきシート名
        private const string SHEET_NAME = "ToDoListMetro";

        // インポートデータに存在するべき列名
        private const string COLUMN_NAME_DUE_DATE = "期日";
        private const string COLUMN_NAME_STATUS_NAME = "状況";
        private const string COLUMN_NAME_SUBJECT = "内容";

        /// <summary>
        /// インポートデータの検証を行います
        /// </summary>
        public static List<CheckImportSheetsResult> CheckImportSheets(DataSet _importSheets)
        {
            List<CheckImportSheetsResult> ret = new List<CheckImportSheetsResult>();

            ret.AddRange(CheckImportSheetNames(_importSheets));

            if (ret.Count != 0) return ret; // これまでの検証でNG個所があれば、ここで処理を終了する

            ret.AddRange(CheckImportSheetFormat(_importSheets));

            if (ret.Count != 0) return ret; // これまでの検証でNG個所があれば、ここで処理を終了する

            ret.AddRange(CheckImportSheetValue(_importSheets));

            return ret;
        }

        /// <summary>
        /// インポートデータに存在するシート名をチェックします
        /// </summary>
        private static List<CheckImportSheetsResult> CheckImportSheetNames(DataSet _importSheets)
        {
            List<CheckImportSheetsResult> result = new List<CheckImportSheetsResult>();

            List<string> importSheetNames = new List<string>();
            foreach (DataTable importSheet in _importSheets.Tables) importSheetNames.Add(importSheet.TableName);

            if (importSheetNames.Contains(SHEET_NAME) == false)
            {
                result.Add(CheckImportSheetsResult.SHEET_NAME_INVALID);
            }

            return result;
        }

        /// <summary>
        /// インポートデータのフォーマットチェックを行います
        /// </summary>
        private static List<CheckImportSheetsResult> CheckImportSheetFormat(DataSet _importSheets)
        {
            List<CheckImportSheetsResult> result = new List<CheckImportSheetsResult>();

            DataTable sheet = _importSheets.Tables[SHEET_NAME];

            // シートにデータが存在すること
            if (sheet.Rows.Count == 0)
            {
                result.Add(CheckImportSheetsResult.SHEET_TODO_TASK_ROW_NOT_FOUND);
            }
            else
            {
                // インポートデータの列名リスト
                List<string> importColumnNames = new List<string>();
                for (int colIndex = 0; colIndex < sheet.Columns.Count; colIndex++) importColumnNames.Add(Convert.ToString(sheet.Rows[0][colIndex]));

                // インポートデータに存在するべき列名リスト
                List<string> properColumnNamesOfTodoTask = new List<string>()
                    {
                        COLUMN_NAME_DUE_DATE,
                        COLUMN_NAME_STATUS_NAME,
                        COLUMN_NAME_SUBJECT
                    };

                // シートの列の不足をチェック
                if (UtilLib.IsFirstContainsAllOfSecond(importColumnNames, properColumnNamesOfTodoTask) == false)
                {
                    result.Add(CheckImportSheetsResult.SHEET_TODO_TASK_COLUMN_INVALID);
                }
            }

            return result;
        }

        /// <summary>
        /// インポートデータの値のチェックを行います
        /// </summary>
        private static List<CheckImportSheetsResult> CheckImportSheetValue(DataSet _importSheets)
        {
            List<CheckImportSheetsResult> result = new List<CheckImportSheetsResult>();

            DataTable sheet = UtilLib.ConvertTableFirstRowAsColumnName(_importSheets.Tables[SHEET_NAME]);
            foreach (DataRow row in sheet.Rows)
            {
                string dueDate = Convert.ToString(row[COLUMN_NAME_DUE_DATE]);
                string statusName = Convert.ToString(row[COLUMN_NAME_STATUS_NAME]);
                string subject = Convert.ToString(row[COLUMN_NAME_SUBJECT]);

                DateTime tempDateTime;

                if (!(DateTime.TryParse(dueDate, out tempDateTime))) result.Add(CheckImportSheetsResult.COLUMN_DUE_DATE_INVALID_DATETIME);

                if(!(StatusCode.HasName(statusName))) result.Add(CheckImportSheetsResult.COLUMN_STATUS_NAME_INVALID_CODE_NAME);

                if (string.IsNullOrEmpty(subject)) result.Add(CheckImportSheetsResult.COLUMN_SUBJECT_BLANK);
            }

            return result;
        }
    }
}
