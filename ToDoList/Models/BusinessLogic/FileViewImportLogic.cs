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
    class FileViewImportLogic
    {
        #region クラス内変数・コンストラクタ

        private DataBaseAccessor _dbAccessor_;

        public FileViewImportLogic(DataBaseAccessor _dbAccessor)
        {
            this._dbAccessor_ = _dbAccessor;
        }

        #endregion

        /// <summary>
        /// タスクテーブルのインポートを行います（［内容］の同じレコードが既に存在する場合、追加も上書きもしません）
        /// </summary>
        public void ImportTodoTask(DataSet _importTables)
        {
            DataTable importTable = UtilLib.ConvertTableFirstRowAsColumnName(_importTables.Tables["ToDoListMetro"]);
            this.ImportTodoTask(importTable);
        }

        /// <summary>
        /// タスクテーブルのインポートを行います（［内容］の同じレコードが既に存在する場合、追加も上書きもしません）
        /// </summary>
        private void ImportTodoTask(DataTable _importTable)
        {
            // 一時テーブルにレコードを登録
            foreach (DataRow row in _importTable.Rows)
            {
                TodoTask record = new TodoTask();

                #region レコード各項目の値を設定
                {
                    DateTime d;
                    if (DateTime.TryParse(Convert.ToString(row["期日"]), out d)) record.DueDate = d;
                }
                record.StatusCode = new StatusCode(StatusCode.GetCodeByName(Convert.ToString(row["状況"])));
                record.Subject = Convert.ToString(row["内容"]);
                #endregion

                _dbAccessor_.TodoTaskInsertWithSkipSameSubject(record);
            }
        }
    }
}
