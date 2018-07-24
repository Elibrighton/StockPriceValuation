using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Services
{
    public class Company
    {
        public string Name { get; set; }
        public string Industry { get; set; }
        public Stock Stock { get; set; }
        public string Sector { get; set; }
    }
}
