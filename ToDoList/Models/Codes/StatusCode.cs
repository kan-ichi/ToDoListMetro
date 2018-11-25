using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.Codes
{
    public class StatusCode
    {
        public const string CODE_NOT_YET = "A";
        public const string CODE_FINISHED = "Z";

        public const string NAME_NOT_YET = "not yet";
        public const string NAME_FINISHED = "finished";

        private static Dictionary<string, string> CodeNamePair = new Dictionary<string, string>()
        {
            {CODE_NOT_YET, NAME_NOT_YET},
            {CODE_FINISHED, NAME_FINISHED}
        };

        private static Dictionary<string, string> NameCodePair = new Dictionary<string, string>()
        {
            {NAME_NOT_YET, CODE_NOT_YET},
            {NAME_FINISHED, CODE_FINISHED}
        };

        public static bool HasName(string _name) { return NameCodePair.ContainsKey(_name); }

        public string Code { get; set; }
        public string Name { get { if (CodeNamePair.Keys.Contains(Code)) return CodeNamePair[Code]; else return string.Empty; } }

        public StatusCode(string _code)
        {
            this.Code = _code;
        }

        public bool IsNotYet { get { return CODE_NOT_YET.Equals(this.Code); } }
        public bool IsFinished { get { return CODE_FINISHED.Equals(this.Code); } }
    }
}
