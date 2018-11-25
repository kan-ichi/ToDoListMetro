using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Codes;
using ToDoList.Models.DataAccess;
using ToDoList.Models.Entities;
using ToDoList.Models.Utilities;

namespace ToDoList.Models.BusinessLogic
{
    class FileViewRestoreLogic
    {
        #region クラス内変数・コンストラクタ

        private DataBaseAccessor _dbAccessor_;

        public FileViewRestoreLogic(DataBaseAccessor _dbAccessor)
        {
            this._dbAccessor_ = _dbAccessor;
        }

        #endregion

        /// <summary>
        /// 各テーブルのリストアを行います（現在のデータは復旧用データに全て置き換わります）
        /// </summary>
        public void RestoreTables(DataSet _restoreTables)
        {
            DataTable todoTaskRestoreTable = UtilLib.ConvertTableFirstRowAsColumnName(_restoreTables.Tables["todo_task"]);
            this.RestoreTableTodoTask(todoTaskRestoreTable);
        }

        /// <summary>
        /// タスクテーブルのリストアを行います（現在のデータは復旧用データに全て置き換わります）
        /// </summary>
        private void RestoreTableTodoTask(DataTable _restoreTable)
        {
            DateTime currentDateTime = DateTime.Now;
            string tempTableName = @"todo_task_temp_" + currentDateTime.ToString("yyyyMMddHHmmssfff");

            // 一時テーブルを作成
            {
                string createTableQuery = _dbAccessor_.GenerateCreateTableQueryTodoTask(tempTableName);
                _dbAccessor_.CreateTable(createTableQuery);
            }

            // 一時テーブルにレコードを登録
            foreach (DataRow row in _restoreTable.Rows)
            {
                TodoTask record = new TodoTask()

                #region レコード各項目の値を設定
                {
                    ID = Convert.ToString(row["id"]),
                    CreatedAt = Convert.ToDateTime(row["created_at"]),
                    UpdatedAt = Convert.ToDateTime(row["updated_at"])
                };
                record.Subject = Convert.ToString(row["subject"]);
                {
                    DateTime d;
                    if (DateTime.TryParse(Convert.ToString(row["due_date"]), out d)) record.DueDate = d;
                }
                record.StatusCode = new StatusCode(Convert.ToString(row["status_code"]));
                #endregion

                _dbAccessor_.TodoTaskInsert(record, tempTableName);
            }

            // 一時テーブルをリネームし、タスクテーブルとする
            _dbAccessor_.RenameTempTableToTodoTask(tempTableName);
        }

    }
}
