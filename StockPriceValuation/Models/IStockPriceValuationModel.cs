using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Models
{
    public interface IStockPriceValuationModel
    {
        int Years { get; set; }
        double RateOfReturn { get; set; }
        double MarginOfSafety { get; set; }
        List<string> Industries { get; set; }
        List<string> Sectors { get; set; }
        int ProgressBarValue { get; set; }
        int ProgressBarMax { get; set; }
        bool ProgressBarIsIndeterminate { get; set; }
    }
}
