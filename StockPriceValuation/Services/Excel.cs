using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace StockPriceValuation.Services
{
    public class Excel
    {
        private Application _Excel;
        private Workbook _Workbook;
        private string _Path;

        public Worksheet Worksheet { get; set; }

        public Excel(string path)
        {
            _Path = path;
            _Excel = new Application();
        }

        public Range GetRange(int firstUsedRow, int firstUsedColumn)
        {
            // open spreadsheet
            _Excel = new Application();
            _Workbook = _Excel.Workbooks.Open(_Path);
            Worksheet = (Worksheet)_Workbook.Worksheets[1];

            var lastUsedRow = Worksheet.Cells.Find("*", System.Reflection.Missing.Value,
                           System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                           XlSearchOrder.xlByRows, XlSearchDirection.xlPrevious,
                           false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

            var lastUsedColumn = Worksheet.Cells.Find("*", System.Reflection.Missing.Value,
                                            System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                            XlSearchOrder.xlByColumns, XlSearchDirection.xlPrevious,
                                            false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Column;

            var startRange = Worksheet.Cells[4, 1];
            var endRange = Worksheet.Cells[lastUsedRow, lastUsedColumn];

            return (Range)Worksheet.Range[startRange, endRange];
        }

        public void Close()
        {
            _Excel.Workbooks.Close();
            _Excel.Quit();

            Marshal.ReleaseComObject(Worksheet);
            Marshal.ReleaseComObject(_Workbook);
            Marshal.ReleaseComObject(_Excel);
        }
    }
}
