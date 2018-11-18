using StockPriceValuation.Services.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Services
{
    public class Stock
    {
        public string Code { get; set; }
        public Exchange StockExchange { get; set; }
        public string Decision { get; set; }
        public double Price { get; set; }
        public bool HasPrice { get; set; }
        public double TtmEps { get; set; }
        public bool HasTtmEps { get; set; }
        public double Eps { get; set; }
        public bool HasEps { get; set; }
        public double PeRatio { get; set; }
        public bool HasPeRatio { get; set; }
        public Valuation Valuation { get; set; }
        public ValueSet FirstEps { get; set; }
        public ValueSet SecondEps { get; set; }
        public ValueSet FirstPeRatio { get; set; }
        public ValueSet SecondPeRatio { get; set; }

        public Stock()
        {
            FirstEps = new ValueSet();
            SecondEps = new ValueSet();
            FirstPeRatio = new ValueSet();
            SecondPeRatio = new ValueSet();
        }

        public void GetYahooFinanceResponse()
        {
            var url = GetYahooFinanceUrl(Code);

            using (var stream = Web.GetStream(url))
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.Load(stream, Encoding.UTF8);

                if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                {
                    // Handle any parse errors as required
                }
                else
                {
                    if (htmlDoc.DocumentNode != null)
                    {
                        HtmlAgilityPack.HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                        if (bodyNode != null)
                        {
                            var spanNodes = htmlDoc.DocumentNode.SelectNodes("//span");

                            if (spanNodes != null)
                            {
                                var hasFoundSymbol = true;

                                foreach (var node in spanNodes)
                                {
                                    if (node.InnerText == string.Concat("Symbols similar to '"))
                                    {
                                        hasFoundSymbol = false;
                                    }
                                }

                                if (hasFoundSymbol)
                                {
                                    foreach (var node in spanNodes)
                                    {
                                        if (!HasPrice)
                                        {
                                            GetPrice(node);
                                        }

                                        if (!HasTtmEps)
                                        {
                                            GetTtmEps(node);
                                        }

                                        if (HasPrice && HasTtmEps)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            url = GetYahooFinanceUrl(Code, true);

            // get second eps
            using (var stream = Web.GetStream(url))
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.Load(stream, Encoding.UTF8);

                if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                {
                    // Handle any parse errors as required
                }
                else
                {
                    if (htmlDoc.DocumentNode != null)
                    {
                        HtmlAgilityPack.HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                        if (bodyNode != null)
                        {
                            var spanNodes = htmlDoc.DocumentNode.SelectNodes("//span");

                            if (spanNodes != null)
                            {
                                var hasFoundSymbol = true;

                                foreach (var node in spanNodes)
                                {
                                    if (node.InnerText == string.Concat("Symbols similar to '") || node.InnerText == string.Concat("Analyst estimates are not available."))
                                    {
                                        hasFoundSymbol = false;
                                    }
                                }

                                if (hasFoundSymbol)
                                {
                                    foreach (var node in spanNodes)
                                    {
                                        if (!SecondEps.HasValue)
                                        {
                                            GetSecondEps(node);
                                        }

                                        if (SecondEps.HasValue)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetPrice(HtmlAgilityPack.HtmlNode node)
        {
            if (node.InnerText.Contains("Ask", StringComparison.OrdinalIgnoreCase))
            {
                var parentNode = node.ParentNode.ParentNode;

                if (parentNode.Name.Contains("tr", StringComparison.OrdinalIgnoreCase))
                {
                    var childrenNodes = parentNode.SelectNodes("td");

                    foreach (var childNode in childrenNodes)
                    {
                        var childSpanNodes = childNode.SelectNodes("span");

                        if (childSpanNodes != null)
                        {
                            foreach (var childTwoNode in childSpanNodes)
                            {
                                var innerText = childTwoNode.InnerText;

                                if (!string.IsNullOrEmpty(innerText) && !childTwoNode.InnerText.Contains("Ask", StringComparison.OrdinalIgnoreCase))
                                {
                                    double convertedInnerText;
                                    var index = innerText.IndexOf('x');

                                    if (index > 0)
                                    {
                                        innerText = innerText.Substring(0, index - 1);
                                    }

                                    if (Double.TryParse(innerText, out convertedInnerText))
                                    {
                                        Price = convertedInnerText;
                                        HasPrice = true;
                                        break;
                                    }
                                }
                            }

                            if (HasPrice)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private string GetYahooFinanceUrl(string code, bool isAnalysisTab = false)
        {
            var isAusStock = StockExchange == Exchange.ASX;

            return string.Concat("https://", isAusStock ? "au." : "", "finance.yahoo.com/quote/", code, isAusStock ? ".AX" : "", isAnalysisTab ? "/analysis" : "");
        }

        public void GetTtmEps(HtmlAgilityPack.HtmlNode node)
        {
            if (node.InnerText.Contains("EPS (TTM)", StringComparison.OrdinalIgnoreCase))
            {
                var parentNode = node.ParentNode.ParentNode;

                if (parentNode.Name.Contains("tr", StringComparison.OrdinalIgnoreCase))
                {
                    var childrenNodes = parentNode.SelectNodes("td");

                    foreach (var childNode in childrenNodes)
                    {
                        var childSpanNodes = childNode.SelectNodes("span");

                        foreach (var childTwoNode in childSpanNodes)
                        {
                            var innerText = childTwoNode.InnerText;

                            if (!string.IsNullOrEmpty(innerText) && !childTwoNode.InnerText.Contains("EPS (TTM)", StringComparison.OrdinalIgnoreCase))
                            {
                                double convertedInnerText;

                                if (Double.TryParse(innerText, out convertedInnerText))
                                {
                                    TtmEps = convertedInnerText;
                                    HasTtmEps = true;
                                    break;
                                }
                            }
                        }

                        if (HasTtmEps)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void GetSecondEps(HtmlAgilityPack.HtmlNode node)
        {
            if (node.InnerText.Contains("Next 5 years (per annum)", StringComparison.OrdinalIgnoreCase))
            {
                var parentNode = node.ParentNode.ParentNode;
                var childrenNodes = parentNode.SelectNodes("td");

                foreach (var childNode in childrenNodes)
                {
                    var innerText = childNode.InnerText;

                    if (!string.IsNullOrEmpty(innerText) && !innerText.Contains("Next 5 years (per annum)", StringComparison.OrdinalIgnoreCase))
                    {
                        innerText = innerText.Replace("%", "");
                        double convertedInnerText;

                        if (Double.TryParse(innerText, out convertedInnerText))
                        {
                            SecondEps.Value = convertedInnerText / 100;
                            SecondEps.HasValue = true;
                        }

                        break; // break after first column so it doesnt get value from another column
                    }
                }
            }
        }

        public void GetWallStreetJournalResponse()
        {
            var url = GetWallStreetJournalUrl(Code);

            using (var stream = Web.GetStream(url))
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.Load(stream, Encoding.UTF8);

                if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                {
                    // Handle any parse errors as required
                }
                else
                {
                    if (htmlDoc.DocumentNode != null)
                    {
                        HtmlAgilityPack.HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                        if (bodyNode != null)
                        {
                            var tdNodes = htmlDoc.DocumentNode.SelectNodes("//td");

                            if (tdNodes != null)
                            {
                                foreach (var node in tdNodes)
                                {
                                    if (node.InnerText.Contains("Total Equity", StringComparison.OrdinalIgnoreCase))
                                    {
                                        var parentNode = node.ParentNode;
                                        if (parentNode.Name == "tr")
                                        {
                                            var equities = new List<double>();
                                            var childrenNodes = parentNode.SelectNodes("td");

                                            foreach (var childNode in childrenNodes)
                                            {
                                                var innerText = childNode.InnerText;

                                                if (!string.IsNullOrWhiteSpace(innerText) && !innerText.Contains("Total Equity", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    innerText = innerText.Replace(",", "");
                                                    double convertedInnerText;

                                                    if (Double.TryParse(innerText, out convertedInnerText))
                                                    {
                                                        equities.Add(convertedInnerText);
                                                    }
                                                }
                                            }

                                            if (equities.Count == 5)
                                            {
                                                double currentEquity = equities[0];
                                                var ageEquity = equities.Count - 1;
                                                double initialEquity = equities[ageEquity];

                                                // calculate equity growth percent https://www.wikihow.com/Calculate-Growth-Rate
                                                FirstEps.Value = Math.Pow((currentEquity / initialEquity), (1 / Convert.ToDouble(ageEquity))) - 1;
                                                FirstEps.HasValue = true;
                                            }
                                        }
                                    }

                                    if (FirstEps.HasValue)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public string GetWallStreetJournalUrl(string code)
        {
            return string.Concat("https://quotes.wsj.com/", StockExchange == Exchange.ASX ? "AU/XASX/" : "", code, "/financials/annual/balance-sheet");
        }

        public void GetEps()
        {
            if (SecondEps.HasValue && FirstEps.HasValue)
            {
                Eps = FirstEps.Value < SecondEps.Value ? FirstEps.Value : SecondEps.Value;
            }
            else if (FirstEps.HasValue)
            {
                Eps = FirstEps.Value;
            }
            else if (SecondEps.HasValue)
            {
                Eps = SecondEps.Value;
            }

            HasEps = FirstEps.HasValue && SecondEps.HasValue;
        }

        public void GetFirstPeRatio()
        {
            var valueSet = new ValueSet();
            FirstPeRatio.Value = (Eps * 100) * 2;
            FirstPeRatio.HasValue = HasEps;
        }

        public void GetMsnMoneyResponse()
        {
            var highPeRatio = 0.0;
            var lowPeRatio = 0.0;
            // how do i get around the number change?
            //https://www.msn.com/en-au/money/stockdetails/analysis/fi-146.1.ATV.NAS
            //https://www.msn.com/en-au/money/stockdetails/analysis/fi-126.1.ATV.NAS
            var url = GetMsnMoneyUrl(Code);

            using (var stream = Web.GetStream(url))
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.Load(stream, Encoding.UTF8);

                if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                {
                    // Handle any parse errors as required
                }
                else
                {
                    if (htmlDoc.DocumentNode != null)
                    {
                        HtmlAgilityPack.HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                        if (bodyNode != null)
                        {
                            var pNodes = htmlDoc.DocumentNode.SelectNodes("//p");

                            if (pNodes != null)
                            {
                                var hasHighPeRatio = false;
                                var hasLowPeRatio = false;

                                foreach (var node in pNodes)
                                {
                                    if (node.InnerText.Contains("P/E Ratio 5-Year High", StringComparison.OrdinalIgnoreCase))
                                    {
                                        var parentNode = node.ParentNode.ParentNode.ParentNode;
                                        var childrenNodes = parentNode.SelectNodes("li");

                                        foreach (var childNode in childrenNodes)
                                        {
                                            var childPNodes = childNode.SelectNodes("p");

                                            foreach (var childTwoNode in childrenNodes)
                                            {
                                                var innerText = childTwoNode.InnerText;

                                                if (!string.IsNullOrEmpty(innerText) && !childTwoNode.InnerText.Contains("P/E Ratio 5-Year High", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    double convertedInnerText;

                                                    if (Double.TryParse(innerText, out convertedInnerText))
                                                    {
                                                        highPeRatio = convertedInnerText;
                                                        hasHighPeRatio = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (hasHighPeRatio)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else if (node.InnerText.Contains("P/E Ratio 5-Year Low", StringComparison.OrdinalIgnoreCase))
                                    {
                                        var parentNode = node.ParentNode.ParentNode.ParentNode;
                                        var childrenNodes = parentNode.SelectNodes("li");

                                        foreach (var childNode in childrenNodes)
                                        {
                                            var childPNodes = childNode.SelectNodes("p");

                                            foreach (var childTwoNode in childrenNodes)
                                            {
                                                var innerText = childTwoNode.InnerText;

                                                if (!string.IsNullOrEmpty(innerText) && !childTwoNode.InnerText.Contains("P/E Ratio 5-Year Low", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    double convertedInnerText;

                                                    if (Double.TryParse(innerText, out convertedInnerText))
                                                    {
                                                        lowPeRatio = convertedInnerText;
                                                        hasLowPeRatio = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (hasLowPeRatio)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (hasHighPeRatio && hasLowPeRatio)
                                    {
                                        // return average of high and low pe ratio
                                        SecondPeRatio.Value = (highPeRatio + lowPeRatio) / 2;
                                        SecondPeRatio.HasValue = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetPeRatio()
        {
            GetFirstPeRatio();

            if (SecondPeRatio.HasValue && FirstPeRatio.HasValue)
            {
                PeRatio = FirstPeRatio.Value < SecondPeRatio.Value ? FirstPeRatio.Value : SecondPeRatio.Value;
            }
            else if (FirstPeRatio.HasValue)
            {
                PeRatio = FirstPeRatio.Value;
            }
            else if (SecondPeRatio.HasValue)
            {
                PeRatio = SecondPeRatio.Value;
            }

            HasPeRatio = FirstPeRatio.HasValue && SecondPeRatio.HasValue;
        }

        public string GetMsnMoneyUrl(string code)
        {
            var isAusStock = StockExchange == Exchange.ASX;

            return string.Concat("https://www.msn.com/en-au/money/stockdetails/analysis/fi-", isAusStock ? "146" : "126", ".1.", code, isAusStock ? ".ASX" : ".NAS");
        }

        public void GetDecision()
        {
            if (Price > 0 && Valuation.BuyPrice > 0)
            {
                if (Price <= Valuation.BuyPrice)
                {
                    Decision = "Buy";
                }
                else if (Price >= (Valuation.FairPrice + (Valuation.FairPrice * 0.2))) // price is greater than 20% of buy price
                {
                    Decision = "Sell";
                }
                else
                {
                    Decision = "Hold";
                }
            }
            else
            {
                Decision = "Unknown";
            }
        }

        public enum Exchange
        {
            ASX,
            NYSE,
            NASDAQ
        }
    }
}
