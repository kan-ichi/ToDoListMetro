using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Utilities
{
    static class UtilLib
    {
        /// <summary>
        /// 最初の一行をカラム名として用い、データテーブルを再作成します（最初の一行は削除されます）
        /// </summary>
        public static DataTable ConvertTableFirstRowAsColumnName(DataTable _inputTable)
        {
            DataTable retTable = new DataTable();
            List<string> columnNames = new List<string>();

            if (_inputTable.Rows.Count > 0)
            {
                for (int colIndex = 0; colIndex < _inputTable.Columns.Count; colIndex++)
                {
                    columnNames.Add(_inputTable.Rows[0][colIndex].ToString());
                }
            }

            if (columnNames.Count != _inputTable.Columns.Count) throw new IndexOutOfRangeException("最初の一行にカラム名が重複して存在しています");

            foreach (string columnName in columnNames)
            {
                retTable.Columns.Add(columnName);
            }

            foreach (DataRow inputRow in _inputTable.Rows)
            {
                if (inputRow == _inputTable.Rows[0]) continue;

                DataRow retRow = retTable.NewRow();
                retRow.ItemArray = inputRow.ItemArray;
                retTable.Rows.Add(retRow);
            }

            return retTable;
        }

        /// <summary>
        /// 引数リスト同士が一致しているかを判定します
        /// </summary>
        public static bool IsMatched(List<string> _firstList, List<string> _secondList)
        {
            bool unMatched = false;
            foreach (var first in _firstList)
            {
                if (_secondList.Contains(first) == false) unMatched = true;
            }
            foreach (var second in _secondList)
            {
                if (_firstList.Contains(second) == false) unMatched = true;
            }
            return !(unMatched);
        }
    }
}
