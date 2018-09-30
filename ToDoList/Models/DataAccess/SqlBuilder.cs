using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.DataAccess
{
    public class SqlBuilder
    {
        public string CommandText { get; private set; }
        public List<SQLiteParameter> Parameters { get; private set; }

        public struct MainViewSearchConditions
        {
            public string StatusCodeNotEqual { get; set; }
        }

        public SqlBuilder(MainViewSearchConditions _cond)
        {
            this.Parameters = new List<SQLiteParameter>();
            this.CommandText = @"SELECT * FROM todo_task WHERE status_code <> @status_code ORDER BY due_date, created_at";
            this.Parameters.Add(new SQLiteParameter(@"@status_code", _cond.StatusCodeNotEqual));
        }

        public struct EditViewSearchConditions
        {
            public string Text { get; set; }
            public DateTime? DueDateFrom { get; set; }
            public DateTime? DueDateTo { get; set; }
        }

        public SqlBuilder(EditViewSearchConditions _cond)
        {
            this.Parameters = new List<SQLiteParameter>();
            string whereClause;
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@" WHERE 1 = 1 ");
                if (!(string.IsNullOrEmpty(_cond.Text)))
                {
                    sb.Append(@" AND subject LIKE @subject ");
                    this.Parameters.Add(new SQLiteParameter(@"@subject", @"%" + _cond.Text + @"%"));
                }
                if (_cond.DueDateFrom.HasValue)
                {
                    sb.Append(@" AND @due_date_from <= DATE(due_date) ");
                    this.Parameters.Add(new SQLiteParameter(@"@due_date_from", _cond.DueDateFrom.Value.Date.ToString("yyyy-MM-dd")));
                }
                if (_cond.DueDateTo.HasValue)
                {
                    sb.Append(@" AND DATE(due_date) <= @due_date_to ");
                    this.Parameters.Add(new SQLiteParameter(@"@due_date_to", _cond.DueDateTo.Value.Date.ToString("yyyy-MM-dd")));
                }
                whereClause = sb.ToString();
            }

            this.CommandText = @"SELECT * FROM todo_task" + whereClause + @" ORDER BY due_date, created_at";

        }
    }
}
