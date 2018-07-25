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
    public abstract class Company
    {
        public string Name { get; set; }
        public string Industry { get; set; }
    }
}
