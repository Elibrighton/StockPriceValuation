﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Models
{
    public class StockPriceValuationModel
    {
        public string PriceToBuyContent { get; set; }
        public List<string> Industries { get; set; }
        public List<string> Sectors { get; set; }
        public int Years { get; set; }
        public double RateOfReturn { get; set; }
        public double MarginOfSafety { get; set; }

        public StockPriceValuationModel()
        {
            Industries = new List<string>();
            Sectors = new List<string>();
        }
    }
}
