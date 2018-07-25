using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Services
{
    public class UsaCompany : Company
    {
        public UsaStock Stock { get; set; }
        public string Sector { get; set; }

        public UsaCompany()
        {
            Stock = new UsaStock();
        }
    }
}
