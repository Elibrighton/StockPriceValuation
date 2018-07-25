using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Services
{
    public abstract class Stock
    {
        public string Code { get; set; }
        public Exchange StockExchange { get; set; }
        public string Decision { get; set; }

        public enum Exchange
        {
            ASX,
            NYSE,
            NASDAQ
        }
    }

}
