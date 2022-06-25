using StockPriceValuation.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        public string ProgressMessage { get; set; }
        public int ProgressBarWidth { get; set; }
        public ObservableCollection<Company> Companies { get; set; }

        public StockPriceValuationModel()
        {
            Industries = new List<string>();
            Sectors = new List<string>();
            Years = 10;
            RateOfReturn = 0.15; // 15%
            MarginOfSafety = 0.50; // 50%
            Companies = new ObservableCollection<Company>();
        }
    }
}
