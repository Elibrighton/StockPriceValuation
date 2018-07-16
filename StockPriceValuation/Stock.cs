using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation
{
    public class Stock
    {
        public string Company { get; set; }
        public string Code { get; set; }
        public double TtmEps { get; set; }
        public double Eps { get; set; }
        public double PeRation { get; set; }
    }
}
