using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation
{
    public class Valuation
    {
        private int _years;
        private double _rateOfReturn;
        private double _marginOfSafetly;

        public double FairPrice { get; set; }
        public double BuyPrice { get; set; }
        public double TtmEps { get; set; }
        public double Eps { get; set; }
        public double PeRatio { get; set; }

        public Valuation(int years, double rateOfReturn, double marginOfSafety)
        {
            _years = years;
            _rateOfReturn = rateOfReturn;
            _marginOfSafetly = marginOfSafety;
        }

        public void GetValuation()
        {
            var epsIn10Years = EpsIn10Years(TtmEps, Eps, _years);
            var sharePriceIn10Years = SharePriceIn10Years(epsIn10Years, PeRatio);
            var yearsToDoubleMoney = YearsToDoubleMoney(_rateOfReturn);
            var numberOfYearsToDoubleMoney = NumberOfYearsToDoubleMoney(_years, yearsToDoubleMoney);

            FairPrice = Math.Round(PriceToBuySharesAtRateOfReturn(numberOfYearsToDoubleMoney, sharePriceIn10Years), 4);
            BuyPrice = Math.Round(PricetoBuySharesAtMarginOfSafety(FairPrice, _marginOfSafetly), 4);
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
