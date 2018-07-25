using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Services
{
    public class AusCompany : Company
    {
        public AusStock Stock { get; set; }

        public AusCompany()
        {
            Stock = new AusStock();
        }
    }
}
