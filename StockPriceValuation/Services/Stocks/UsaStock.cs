using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Services
{
    public class UsaStock : Stock
    {
        public List<Sale> Sales { get; set; }
        public double Eps2008 { get; set; }
        public double Eps2009 { get; set; }
        public double Eps2010 { get; set; }
        public double Eps2011 { get; set; }
        public double Eps2012 { get; set; }
        public double Eps2013 { get; set; }
        public double Eps2014 { get; set; }
        public double Eps2015 { get; set; }
        public double Eps2016 { get; set; }
        public double Eps2017 { get; set; }
        public double Equity2008 { get; set; }
        public double Equity2009 { get; set; }
        public double Equity2010 { get; set; }
        public double Equity2011 { get; set; }
        public double Equity2012 { get; set; }
        public double Equity2013 { get; set; }
        public double Equity2014 { get; set; }
        public double Equity2015 { get; set; }
        public double Equity2016 { get; set; }
        public double Equity2017 { get; set; }
        public double Fcf2008 { get; set; }
        public double Fcf2009 { get; set; }
        public double Fcf2010 { get; set; }
        public double Fcf2011 { get; set; }
        public double Fcf2012 { get; set; }
        public double Fcf2013 { get; set; }
        public double Fcf2014 { get; set; }
        public double Fcf2015 { get; set; }
        public double Fcf2016 { get; set; }
        public double Fcf2017 { get; set; }
        public double CurrentAssets { get; set; }
        public double CurrentLiabilites { get; set; }
        public double TotalLiabilities { get; set; }
        public bool HasSales { get; set; }

        public UsaStock()
        {
            Sales = new List<Sale>();
        }

        public void GetSales()
        {
            var url = GetSalesUrl(Code);

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
                                var year = 2008;

                                foreach (var node in tdNodes)
                                {
                                    if (node.InnerText == "Revenue")
                                    {
                                        var parentNode = node.ParentNode.ParentNode;

                                        if (parentNode.Name == "tr")
                                        {
                                            var childrenNodes = parentNode.SelectNodes("td");

                                            foreach (var childNode in childrenNodes)
                                            {
                                                var childTrNodes = childNode.SelectNodes("tr");

                                                if (childTrNodes != null)
                                                {
                                                    foreach (var childTwoNode in childTrNodes)
                                                    {
                                                        var innerText = childTwoNode.InnerText;

                                                        if (!string.IsNullOrEmpty(innerText) && !childTwoNode.InnerText.Contains("Revenue"))
                                                        {
                                                            double convertedInnerText;

                                                            if (Double.TryParse(innerText.Replace(",", ""), out convertedInnerText))
                                                            {
                                                                var sales = new Sale();
                                                                sales.Value = convertedInnerText;
                                                                sales.Year = year;
                                                                Sales.Add(sales);

                                                                HasSales = Sales.Count == 10;
                                                                year++;
                                                            }
                                                        }
                                                    }

                                                    if (HasSales)
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
                }
            }
        }


        private string GetSalesUrl(string code)
        {
            return string.Concat("https://quickfs.net/company/", code);
        }
    }
}
