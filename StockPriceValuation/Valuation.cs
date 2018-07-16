using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation
{
    public class Valuation
    {
        private Stock _stock;

        public int Years { get; set; }
        public double RateOfReturn { get; set; }
        public double MarginOfSafety { get; set; }

        public Valuation (Stock stock)
        {
            _stock = stock;
        }

        public double ValueStockPrice()
        {
            var stockPriceValue = 0.0;

            var epsIn10Years = EpsIn10Years(_stock.TtmEps, _stock.Eps, Years);
            var sharePriceIn10Years = SharePriceIn10Years(epsIn10Years, _stock.PeRation);
            var yearsToDoubleMoney = YearsToDoubleMoney(RateOfReturn);
            var numberOfYearsToDoubleMoney = NumberOfYearsToDoubleMoney(Years, yearsToDoubleMoney);
            var priceToBuySharesAtRateOfReturn = PriceToBuySharesAtRateOfReturn(numberOfYearsToDoubleMoney, sharePriceIn10Years);
            var pricetoBuySharesAtMarginOfSafety = PricetoBuySharesAtMarginOfSafety(priceToBuySharesAtRateOfReturn, MarginOfSafety);
            stockPriceValue = pricetoBuySharesAtMarginOfSafety;

            return stockPriceValue;
        }

        public double EpsIn10Years(double ttmEps, double eps, double years)
        {
            return ttmEps * (Math.Pow(1 + eps, years));
        }

        public double SharePriceIn10Years(double epsIn10Years, double peRatio)
        {
            return epsIn10Years * peRatio;
        }

        public double YearsToDoubleMoney(double rateOfReturn)
        {
            // using rule of 72
            return 72 / (rateOfReturn * 100);
        }

        public double NumberOfYearsToDoubleMoney(double years, double yearsToDoubleMoney)
        {
            return years / yearsToDoubleMoney;
        }

        public double PriceToBuySharesAtRateOfReturn(double numberOfYearsToDoubleMoney, double sharePriceIn10Years)
        {
            return sharePriceIn10Years / Math.Pow(2, numberOfYearsToDoubleMoney);
        }

        public double PricetoBuySharesAtMarginOfSafety(double priceToBuySharesAtRateOfReturn, double marginOfSafety)
        {
            return priceToBuySharesAtRateOfReturn * marginOfSafety;
        }
    }
}
