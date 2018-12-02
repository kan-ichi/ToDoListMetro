using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Codes;
using ToDoList.Models.Entities;

namespace ToDoList.Models.DataAccess
{
    /// <summary>
    /// データベースアクセス管理クラス
    /// </summary>
    public class DataBaseAccessor
    {
        public const string DB_FILENAME = @"ToDoList.db";
        private readonly string _connectionString_ = null;
        private readonly string _dbFilePath_ = System.AppDomain.CurrentDomain.BaseDirectory + DB_FILENAME;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataBaseAccessor()
        {
            var builder = new SQLiteConnectionStringBuilder()
            {
                DataSource = DB_FILENAME,
                Version = 3
            };
            _connectionString_ = builder.ToString();
        }

        /// <summary>
        /// このシステムで使用するデータベースが存在するかを確認します
        /// </summary>
        public bool IsExistDataBaseFile()
        {
            return System.IO.File.Exists(_dbFilePath_);
        }

        /// <summary>
        /// このシステムで使用するテーブルを作成します
        /// </summary>
        public void CreateDataBase()
        {
            this.CreateTable(this.GenerateCreateTableQueryTodoTask());
        }

        /// <summary>
        /// データベースにテーブルを作成します
        /// </summary>
        public void CreateTable(string _createTableQuery)
        {
            using (var connection = new SQLiteConnection(_connectionString_))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = _createTableQuery;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// タスクテーブルのCREATEクエリを生成します
        /// </summary>
        public string GenerateCreateTableQueryTodoTask()
        {
            return this.GenerateCreateTableQueryTodoTask(@"todo_task");
        }

        /// <summary>
        /// タスクテーブルのCREATEクエリを生成します（テーブル名として任意の文字列を指定可能）
        /// </summary>
        public string GenerateCreateTableQueryTodoTask(string _tableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"CREATE TABLE ");
            sb.Append(_tableName);
            sb.Append(@"(id TEXT PRIMARY KEY, created_at TEXT NOT NULL, updated_at TEXT NOT NULL");
            sb.Append(@", subject      TEXT");
            sb.Append(@", due_date     TEXT");
            sb.Append(@", status_code  TEXT NOT NULL");
            sb.Append(@")");
            return sb.ToString();
        }

        /// <summary>
        /// 一時テーブルをリネームし、タスクテーブルとします（元のタスクテーブルは削除します）
        /// </summary>
        public void RenameTempTableToTodoTask(string _tempTableName)
        {
            DateTime currentDateTime = DateTime.Now;
            string deleteTableName = @"todo_task_temp_del_" + currentDateTime.ToString("yyyyMMddHHmmssfff");

            using (var connection = new SQLiteConnection(_connectionString_))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"ALTER TABLE todo_task RENAME TO " + deleteTableName;
                    command.ExecuteNonQuery();
                    command.CommandText = @"ALTER TABLE " + _tempTableName + @" RENAME TO todo_task";
                    command.ExecuteNonQuery();
                    command.CommandText = @"DROP TABLE " + deleteTableName;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// データベースに存在するテーブルの一覧を取得します
        /// </summary>
        public List<string> GetTableNameList()
        {
            List<string> ret = new List<string>();
            using (var connection = new SQLiteConnection(_connectionString_))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read() == true)
                    {
                        ret.Add(Convert.ToString(reader["name"]));
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// テーブル内の全てのレコードを取得します
        /// </summary>
        public DataTable SelectAll(string _tableName)
        {
            DataTable retTable = new DataTable(_tableName);
            List<string> columnNames = new List<string>();

            using (var connection = new SQLiteConnection(_connectionString_))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"PRAGMA TABLE_INFO(" + _tableName + ")";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read() == true)
                    {
                        columnNames.Add(Convert.ToString(reader["name"]));
                    }
                }
            }

            foreach(string columnName in columnNames)
            {
                retTable.Columns.Add(columnName);
            }

            using (var connection = new SQLiteConnection(_connectionString_))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"SELECT * FROM " + _tableName;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read() == true)
                    {
                        DataRow dr = retTable.NewRow();
                        foreach (string columnName in columnNames)
                        {
                            dr[columnName] = reader[columnName];
                        }
                        retTable.Rows.Add(dr);
                    }
                }
            }

            return retTable;
        }

        /// <summary>
        /// タスクテーブルからレコードを取得します
        /// </summary>
        public List<TodoTask> TodoTaskSelect(SqlBuilder _sql)
        {
            var records = new List<TodoTask>();

            using (var connection = new SQLiteConnection(_connectionString_))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = _sql.CommandText;
                command.Parameters.AddRange(_sql.Parameters.ToArray());

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read() == true)
                    {
                        TodoTask record = new TodoTask()
                        {
                            ID = Convert.ToString(reader["id"]),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            UpdatedAt = Convert.ToDateTime(reader["updated_at"])
                        };
                        record.Subject = Convert.ToString(reader["subject"]);
                        {
                            DateTime d;
                            if (DateTime.TryParse(Convert.ToString(reader["due_date"]), out d)) record.DueDate = d;
                        }
                        record.StatusCode = new StatusCode(Convert.ToString(reader["status_code"]));

                        records.Add(record);
                    }
                }
            }
            return records;
        }

        /// <summary>
        /// タスクテーブルにレコードを追加します
        /// </summary>
        public void TodoTaskInsert(TodoTask _record, string alias = "todo_task")
        {
            var id = this.GenerateId();
            var now = DateTime.Now;

            _record.CreatedAt = now;
            _record.UpdatedAt = now;

            using (var connection = new SQLiteConnection(_connectionString_))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = @"INSERT INTO " + alias + @" " +
                    @"( id,  created_at,  updated_at,  subject,  due_date,  status_code) VALUES " +
                    @"(@id, @created_at, @updated_at, @subject, @due_date, @status_code)";

                command.Parameters.Add(new SQLiteParameter("@id", id));
                command.Parameters.Add(new SQLiteParameter("@created_at", _record.CreatedAt));
                command.Parameters.Add(new SQLiteParameter("@updated_at", _record.UpdatedAt));
                command.Parameters.Add(new SQLiteParameter("@subject", _record.Subject));
                command.Parameters.Add(new SQLiteParameter("@due_date", _record.DueDate));
                command.Parameters.Add(new SQLiteParameter("@status_code", _record.StatusCode.Code));

                command.Prepare(); //パラメータをセット
                command.ExecuteNonQuery();
            }

            _record.ID = id;
        }

        /// <summary>
        /// タスクテーブルにレコードを追加します（［内容］の同じレコードが既に存在する場合、追加も上書きもしません）
        /// </summary>
        public void TodoTaskInsertWithSkipSameSubject(TodoTask _record, string alias = "todo_task")
        {
            var id = this.GenerateId();
            var now = DateTime.Now;

            _record.CreatedAt = now;
            _record.UpdatedAt = now;

            using (var connection = new SQLiteConnection(_connectionString_))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = @"INSERT INTO " + alias + @" " +
                    @"( id,  created_at,  updated_at,  subject,  due_date,  status_code) SELECT " +
                    @"@id, @created_at, @updated_at, @subject, @due_date, @status_code " +
                    @"WHERE NOT EXISTS (SELECT 1 FROM " + alias + @" WHERE subject = @subject)";

                command.Parameters.Add(new SQLiteParameter("@id", id));
                command.Parameters.Add(new SQLiteParameter("@created_at", _record.CreatedAt));
                command.Parameters.Add(new SQLiteParameter("@updated_at", _record.UpdatedAt));
                command.Parameters.Add(new SQLiteParameter("@subject", _record.Subject));
                command.Parameters.Add(new SQLiteParameter("@due_date", _record.DueDate));
                command.Parameters.Add(new SQLiteParameter("@status_code", _record.StatusCode.Code));

                command.Prepare(); //パラメータをセット
                command.ExecuteNonQuery();
            }

            _record.ID = id;
        }

        /// <summary>
        /// タスクテーブルの指定レコードを更新します
        /// </summary>
        public void TodoTaskUpdate(TodoTask _record)
        {
            _record.UpdatedAt = DateTime.Now;

            using (var connection = new SQLiteConnection(_connectionString_))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"UPDATE todo_task SET updated_at = @updated_at" +
                    @", subject      = @subject" +
                    @", due_date     = @due_date" +
                    @", status_code  = @status_code" +
                    @" WHERE id = @id";

                command.Parameters.Add(new SQLiteParameter("@id", _record.ID));
                command.Parameters.Add(new SQLiteParameter("@updated_at", _record.UpdatedAt));
                command.Parameters.Add(new SQLiteParameter("@subject", _record.Subject));
                command.Parameters.Add(new SQLiteParameter("@due_date", _record.DueDate));
                command.Parameters.Add(new SQLiteParameter("@status_code", _record.StatusCode.Code));

                command.Prepare(); //パラメータをセット
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// タスクテーブルの指定レコードを削除します
        /// </summary>
        public void TodoTaskDelete(TodoTask _record)
        {
            this.TodoTaskDelete(_record.ID);
        }

        /// <summary>
        /// タスクテーブルの指定レコードを削除します
        /// </summary>
        public void TodoTaskDelete(string _id)
        {
            using (var connection = new SQLiteConnection(_connectionString_))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = @"DELETE FROM todo_task WHERE id = @id";
                command.Parameters.Add(new SQLiteParameter("@id", _id));
                command.Prepare();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 主キーとして使用する文字列を生成します
        /// </summary>
        private string GenerateId()
        {
            string datePart = DateTime.Now.ToString("yyyyMMddHHmmssff");
            string guidPart = Guid.NewGuid().ToString("N");
            return datePart + guidPart;
        }

    }
}
