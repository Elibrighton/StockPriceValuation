using StockPriceValuation.Base;
using StockPriceValuation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockPriceValuation
{
    public class StockPriceValuationViewModel : ObservableObject
    {
        private StockPriceValuationModel stockPriceValuation = new StockPriceValuationModel();

        private string _lblPriceToBuyContent;

        public string LblPriceToBuyContent
        {
            get { return _lblPriceToBuyContent; }
            set
            {
                _lblPriceToBuyContent = value;
                stockPriceValuation.PriceToBuyContent = _lblPriceToBuyContent;
                NotifyPropertyChanged("LblPriceToBuyContent");
            }
        }

        public ICommand GetPriceToBuyButtonCommand { get; set; }

        public StockPriceValuationViewModel()
        {
            LblPriceToBuyContent = "$0";
            GetPriceToBuyButtonCommand = new RelayCommand(OnGetPriceToBuyButtonCommand);
        }

        private async void OnGetPriceToBuyButtonCommand(object param)
        {
            LblPriceToBuyContent = await Task.Run(() => GetStockPriceValuation().ToString());
        }

        public string GetStockPriceValuation()
        {
            var stock = GetStock();

            var stockPriceValuation = 0.0;

            var valuation = new Valuation(stock);
            valuation.Years = 10;
            valuation.RateOfReturn = 0.15; // 15%
            valuation.MarginOfSafety = 0.50; // 50%
            stockPriceValuation = valuation.ValueStockPrice();

            return string.Concat("$", Math.Round(valuation.ValueStockPrice()));
        }

        public Stock GetStock()
        {
            var stock = new Stock("Facebook, Inc.", "FB");
            //{
            //    TtmEps = 6.04,
            //    Eps = 0.2362,
            //    PeRation = 19.375
            //};
            //stock.FindTtmEps();
            stock.FindEps();
            stock.FindPeRatio();

            return stock;
        }
    }
}
