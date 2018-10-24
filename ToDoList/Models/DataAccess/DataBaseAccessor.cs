using System;
using System.Collections.Generic;
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
            using (var connection = new SQLiteConnection(_connectionString_))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"CREATE TABLE todo_task(id TEXT PRIMARY KEY, created_at TEXT NOT NULL, updated_at TEXT NOT NULL" +
                        @", subject      TEXT" +
                        @", due_date     TEXT" +
                        @", status_code  TEXT NOT NULL" +
                        @")";
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
                        ret.Add(reader["name"].ToString());
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// テーブル内の全てのレコードを取得します（一行目にカラム名を付与して取得）
        /// </summary>
        public List<List<object>> SelectAllWithHeader(string _tableName)
        {
            return this.SelectAll(_tableName, true);
        }

        /// <summary>
        /// テーブル内の全てのレコードを取得します
        /// </summary>
        public List<List<object>> SelectAll(string _tableName, bool _withHeader)
        {
            List<List<object>> recordList = new List<List<object>>();

            if (_withHeader)
            {
                using (var connection = new SQLiteConnection(_connectionString_))
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"PRAGMA TABLE_INFO(" + _tableName + ")";
                    using (var reader = command.ExecuteReader())
                    {
                        List<object> columnArray = new List<object>();
                        while (reader.Read() == true)
                        {
                            columnArray.Add(reader["name"].ToString());
                        }
                        recordList.Add(columnArray);
                    }
                }
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
                        List<object> columnArray = new List<object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columnArray.Add(reader.GetValue(i));
                        }
                        recordList.Add(columnArray);
                    }
                }
            }

            return recordList;
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
                            ID = reader["id"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            UpdatedAt = Convert.ToDateTime(reader["updated_at"])
                        };
                        record.Subject = reader["subject"].ToString();
                        {
                            DateTime d;
                            if (DateTime.TryParse(reader["due_date"].ToString(), out d)) record.DueDate = d;
                        }
                        record.StatusCode = new StatusCode(reader["status_code"].ToString());

                        records.Add(record);
                    }
                }
            }
            return records;
        }

        /// <summary>
        /// タスクテーブルにレコードを追加します
        /// </summary>
        public void TodoTaskInsert(TodoTask _record)
        {
            var id = this.GenerateId();
            var now = DateTime.Now;

            _record.CreatedAt = now;
            _record.UpdatedAt = now;

            using (var connection = new SQLiteConnection(_connectionString_))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = @"INSERT INTO todo_task " +
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
