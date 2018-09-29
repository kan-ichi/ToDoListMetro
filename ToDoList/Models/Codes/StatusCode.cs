using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Codes
{
    public class StatusCode
    {
        public const string NOT_YET = "A";
        public const string IN_PROGRESS = "K";
        public const string FINISHED = "Z";

        public static Dictionary<string,string> CodeNamePair = new Dictionary<string, string>()
        {
            {NOT_YET, "not yet"},
            {IN_PROGRESS, "in progress"},
            {FINISHED, "finished"}
        };

        public string Code { get; set; }
        public string Name { get { if (CodeNamePair.Keys.Contains(Code)) return CodeNamePair[Code];  else return string.Empty; } }

        public StatusCode(string _code)
        {
            this.Code = _code;
        }
    }
}
