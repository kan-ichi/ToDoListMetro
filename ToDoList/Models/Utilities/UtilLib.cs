using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Utilities
{
    static class UtilLib
    {
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
