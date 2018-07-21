using StockPriceValuation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockPriceValuation
{
    public class Stock
    {
        public string Code { get; set; }
        public double TtmEps { get; set; }
        public double Eps { get; set; }
        public double PeRatio { get; set; }
        public double Price { get; set; }
        public double FairPrice { get; set; }
        public double BuyPrice { get; set; }
        public Valuation Valuation { get; set; }

        public void GetPrice()
        {
            var url = GetPriceUrl(Code);

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
                                var hasPrice = false;

                                foreach (var node in spanNodes)
                                {
                                    if (node.InnerText == "Ask")
                                    {
                                        var parentNode = node.ParentNode.ParentNode;

                                        if (parentNode.Name == "tr")
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

                                                        if (!string.IsNullOrEmpty(innerText) && !childTwoNode.InnerText.Contains("Ask"))
                                                        {
                                                            double convertedInnerText;

                                                            if (Double.TryParse(innerText.Replace(" x 0", ""), out convertedInnerText))
                                                            {
                                                                Price = convertedInnerText;
                                                                hasPrice = true;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (hasPrice)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (hasPrice)
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

        public void GetTtmEps()
        {
            var url = GetTtmEpsUrl(Code);

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
                                var hasTtmEps = false;

                                foreach (var node in spanNodes)
                                {
                                    if (node.InnerText == "EPS (TTM)")
                                    {
                                        var parentNode = node.ParentNode.ParentNode;

                                        if (parentNode.Name == "tr")
                                        {
                                            var childrenNodes = parentNode.SelectNodes("td");

                                            foreach (var childNode in childrenNodes)
                                            {
                                                var childSpanNodes = childNode.SelectNodes("span");

                                                foreach (var childTwoNode in childSpanNodes)
                                                {
                                                    var innerText = childTwoNode.InnerText;

                                                    if (!string.IsNullOrEmpty(innerText) && !childTwoNode.InnerText.Contains("EPS (TTM)"))
                                                    {
                                                        double convertedInnerText;

                                                        if (Double.TryParse(innerText, out convertedInnerText))
                                                        {
                                                            TtmEps = convertedInnerText;
                                                            hasTtmEps = true;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (hasTtmEps)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (hasTtmEps)
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

        private string GetTtmEpsUrl(string code)
        {
            return string.Concat("https://au.finance.yahoo.com/quote/", code);
        }

        private string GetPriceUrl(string code)
        {
            return string.Concat("https://au.finance.yahoo.com/quote/", code, ".AX");
        }

        public void GetEps()
        {
            var epsFirst = GetFirstEps();
            var epsSecond = GetSecondEps();

            Eps = epsFirst < epsSecond ? epsFirst : epsSecond;
        }

        private double GetFirstEps()
        {
            var eps = 0.0;
            var url = GetFirstEpsUrl(Code);

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
                                var hasEps = false;

                                foreach (var node in tdNodes)
                                {
                                    if (node.InnerText == "Total Equity")
                                    {
                                        var parentNode = node.ParentNode;
                                        if (parentNode.Name == "tr")
                                        {
                                            var equities = new List<int>();
                                            var childrenNodes = parentNode.SelectNodes("td");

                                            foreach (var childNode in childrenNodes)
                                            {
                                                var innerText = childNode.InnerText;

                                                if (!string.IsNullOrWhiteSpace(innerText) && innerText != "Total Equity")
                                                {
                                                    var text = innerText.Replace(",", "");
                                                    equities.Add(Convert.ToInt32(text));
                                                }
                                            }

                                            if (equities.Count > 0)
                                            {
                                                double currentEquity = equities[0];
                                                var ageEquity = equities.Count - 1;
                                                double initialEquity = equities[ageEquity];

                                                // calculate equity growth percent https://www.wikihow.com/Calculate-Growth-Rate
                                                eps = Math.Pow((currentEquity / initialEquity), (1 / Convert.ToDouble(ageEquity))) - 1;
                                                hasEps = true;
                                            }
                                        }
                                    }

                                    if (hasEps)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return eps;
        }

        public double GetSecondEps()
        {
            var eps = 0.0;
            var url = GetSecondEpsUrl(Code);

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
                                var hasEps = false;

                                foreach (var node in spanNodes)
                                {
                                    if (node.InnerText == "Next 5 years (per annum)")
                                    {
                                        var parentNode = node.ParentNode.ParentNode;
                                        var childrenNodes = parentNode.SelectNodes("td");

                                        foreach (var childNode in childrenNodes)
                                        {
                                            var innerText = childNode.InnerText;

                                            if (!string.IsNullOrEmpty(innerText) && innerText != "Next 5 years (per annum)")
                                            {
                                                innerText = innerText.Replace("%", "");
                                                double convertedInnerText;

                                                if (Double.TryParse(innerText, out convertedInnerText))
                                                {
                                                    eps = convertedInnerText / 100;
                                                }

                                                hasEps = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (hasEps)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return eps;
        }

        private string GetFirstEpsUrl(string code)
        {
            return string.Concat("https://quotes.wsj.com/", code, "/financials/annual/balance-sheet");
        }

        private string GetSecondEpsUrl(string code)
        {
            return string.Concat("https://au.finance.yahoo.com/quote/", code, "/analysis");
        }

        private string GetSecondPeRatioUrl(string code)
        {
            return string.Concat("https://www.msn.com/en-au/money/stockdetails/analysis/fi-126.1.", code, ".NAS");
        }

        public void GetPeRatio()
        {
            var firstPeRation = GetFirstPeRatio();
            var secondPeRation = GetSecondPeRatio();

            PeRatio = firstPeRation < secondPeRation ? firstPeRation : secondPeRation;
        }

        public double GetFirstPeRatio()
        {
            return (Eps * 100) * 2;
        }

        public double GetSecondPeRatio()
        {
            var peRatio = 0.0;
            var highPeRatio = 0.0;
            var lowPeRatio = 0.0;
            var url = GetSecondPeRatioUrl(Code);

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
                                    if (node.InnerText == "P/E Ratio 5-Year High")
                                    {
                                        var parentNode = node.ParentNode.ParentNode.ParentNode;
                                        var childrenNodes = parentNode.SelectNodes("li");

                                        foreach (var childNode in childrenNodes)
                                        {
                                            var childPNodes = childNode.SelectNodes("p");

                                            foreach (var childTwoNode in childrenNodes)
                                            {
                                                var innerText = childTwoNode.InnerText;

                                                if (!string.IsNullOrEmpty(innerText) && !childTwoNode.InnerText.Contains("P/E Ratio 5-Year High"))
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
                                    else if (node.InnerText == "P/E Ratio 5-Year Low")
                                    {
                                        var parentNode = node.ParentNode.ParentNode.ParentNode;
                                        var childrenNodes = parentNode.SelectNodes("li");

                                        foreach (var childNode in childrenNodes)
                                        {
                                            var childPNodes = childNode.SelectNodes("p");

                                            foreach (var childTwoNode in childrenNodes)
                                            {
                                                var innerText = childTwoNode.InnerText;

                                                if (!string.IsNullOrEmpty(innerText) && !childTwoNode.InnerText.Contains("P/E Ratio 5-Year Low"))
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
                                        peRatio = (highPeRatio + lowPeRatio) / 2;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return peRatio;
        }
    }
}
