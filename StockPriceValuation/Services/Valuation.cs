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
        private int _years;
        private double _rateOfReturn;
        private double _marginOfSafetly;

        public double FairPrice { get; set; }
        public double BuyPrice { get; set; }

        public Valuation (Stock stock, int years, double rateOfReturn, double marginOfSafety)
        {
            _stock = stock;
            _years = years;
            _rateOfReturn = rateOfReturn;
            _marginOfSafetly = marginOfSafety;
        }

        public void GetValuation()
        {
            var epsIn10Years = EpsIn10Years(_stock.TtmEps, _stock.Eps, _years);
            var sharePriceIn10Years = SharePriceIn10Years(epsIn10Years, _stock.PeRatio);
            var yearsToDoubleMoney = YearsToDoubleMoney(_rateOfReturn);
            var numberOfYearsToDoubleMoney = NumberOfYearsToDoubleMoney(_years, yearsToDoubleMoney);

            FairPrice = PriceToBuySharesAtRateOfReturn(numberOfYearsToDoubleMoney, sharePriceIn10Years);
            BuyPrice = PricetoBuySharesAtMarginOfSafety(FairPrice, _marginOfSafetly);
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
