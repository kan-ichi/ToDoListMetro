using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models.DataAccess
{
    class XlsxWriter
    {
        public XlsxWriter()
        {
        }

        public void WriteTest(string _fileName)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sample Sheet");
            worksheet.Cell("A1").Value = "Hello World!!";
            workbook.SaveAs(_fileName);
        }
    }
}
