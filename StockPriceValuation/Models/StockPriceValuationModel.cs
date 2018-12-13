using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Models
{
    public class StockPriceValuationModel : IStockPriceValuationModel
    {
        public string PriceToBuyContent { get; set; }
        public List<string> Industries { get; set; }
        public List<string> Sectors { get; set; }
        public int Years { get; set; }
        public double RateOfReturn { get; set; }
        public double MarginOfSafety { get; set; }
        public int ProgressBarValue { get; set; }
        public int ProgressBarMax { get; set; }
        public bool ProgressBarIsIndeterminate { get; set; }

        public StockPriceValuationModel()
        {
            Industries = new List<string>();
            Sectors = new List<string>();
            Years = 10;
            RateOfReturn = 0.15; // 15%
            MarginOfSafety = 0.50; // 50%
        }
    }
}
