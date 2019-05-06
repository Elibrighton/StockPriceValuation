using StockPriceValuation.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        string ProgressMessage { get; set; }
        int ProgressBarWidth { get; set; }
        ObservableCollection<Company> Companies { get; set; }
    }
}
