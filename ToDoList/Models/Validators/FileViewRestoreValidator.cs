using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Utilities;

namespace ToDoList.Models.Validators
{
    /// <summary>
    /// FileView データ復旧のリストアデータ検証を行います
    /// </summary>
    static class FileViewRestoreValidator
    {
        /// <summary>
        /// 復旧用データの検証結果
        /// </summary>
        public enum CheckRestoreSheetsResult
        {
            /// <summary>
            /// シート名が不正
            /// </summary>
            SHEET_NAMES_INVALID,

            /// <summary>
            /// todo_task シートに行が存在しない
            /// </summary>
            SHEET_TODO_TASK_ROW_NOT_FOUND,

            /// <summary>
            /// todo_task シートの列に過不足がある
            /// </summary>
            SHEET_TODO_TASK_COLUMN_INVALID
        }

        /// <summary>
        /// 復旧用データの検証を行います
        /// </summary>
        public static List<CheckRestoreSheetsResult> CheckRestoreSheets(Dictionary<string, List<List<object>>> _restoreSheets)
        {
            List<CheckRestoreSheetsResult> ret = new List<CheckRestoreSheetsResult>();

            // 復旧用データに存在するべきシート名
            List<string> properSheetNames = new List<string>()
            {
                "todo_task"
            };

            // 復旧用データ todo_task に存在するべき列名
            List<string> properColumnNamesOfTodoTask = new List<string>()
            {
                "id",
                "created_at",
                "updated_at",
                "subject",
                "due_date",
                "status_code"
            };

            // 復旧用データに存在するシート名をチェック
            {
                List<string> restoreSheetNames = new List<string>();
                foreach (var restoreSheet in _restoreSheets) restoreSheetNames.Add(restoreSheet.Key);

                if (UtilLib.IsMatched(properSheetNames, restoreSheetNames) == false)
                {
                    ret.Add(CheckRestoreSheetsResult.SHEET_NAMES_INVALID);
                    return ret;
                }
            }

            // 復旧用データ todo_task のチェック
            List<List<object>> sheet = _restoreSheets["todo_task"];

            // シートにデータが存在すること
            if (sheet.Count == 0)
            {
                ret.Add(CheckRestoreSheetsResult.SHEET_TODO_TASK_ROW_NOT_FOUND);
            }
            else
            {
                List<string> restoreColumnNames = new List<string>();
                foreach (var firstColumn in sheet[0]) restoreColumnNames.Add(firstColumn.ToString());

                // シートの列の過不足をチェック
                if (UtilLib.IsMatched(properColumnNamesOfTodoTask, restoreColumnNames) == false)
                {
                    ret.Add(CheckRestoreSheetsResult.SHEET_TODO_TASK_COLUMN_INVALID);
                }
            }

            return ret;
        }

    }
}
