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
        private string _company;
        private string _code;

        public double TtmEps { get; set; }
        public double Eps { get; set; }
        public double PeRation { get; set; }

        public Stock(string company, string code)
        {
            _company = company;
            _code = code;
        }

        public void FindTtmEps()
        {
            var ttmEps = string.Empty;
            var url = GetTtmEpsUrl(_code);
            var webResponse = GetWebResponse(url);

            if (!string.IsNullOrEmpty(webResponse))
            {
                //<span class="Trsdu(0.3s) " data-reactid="97">6.04</span>
                var pattern = @"<span\sclass=""trsdu\(0.3s\)\s""\sdata-reactid=""97"">\d+.\d+</span>";
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                var match = regex.Match(webResponse);

                if (match.Success)
                {
                    var ttmEpsMatch = match.Value;
                    var index = ttmEpsMatch.IndexOf('>') + 1;
                    ttmEps = ttmEpsMatch.Substring(index, ttmEpsMatch.LastIndexOf('<') - index);
                    var number = 0.0;

                    if (Double.TryParse(ttmEps, out number))
                    {
                        TtmEps = number;
                    }
                }
            }
        }

        private string GetWebResponse(string url)
        {
            var request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            var response = request.GetResponse();
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            //Console.WriteLine(responseFromServer);
            reader.Close();
            response.Close();

            return responseFromServer;
        }

        private string GetTtmEpsUrl(string code)
        {
            return string.Concat("https://au.finance.yahoo.com/quote/", code);
        }

        public void FindEps()
        {
            var epsFirst = FindFirstEps();
        }

        private double FindFirstEps()
        {
            var eps = 0.0;
            var url = GetEpsUrl(_code);
            var webResponse = GetWebResponse(url);

            if (!string.IsNullOrEmpty(webResponse))
            {
                using (var stream = GenerateStreamFromString(webResponse))
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
                                                    var currentEquity = equities[0];
                                                    var ageEquity = equities.Count - 1;
                                                    var initialEquity = equities[ageEquity];
                                                    // how to calculate equity growth percent?
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

            return eps;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private string GetEpsUrl(string code)
        {
            return string.Concat("https://quotes.wsj.com/", code, "/financials/annual/balance-sheet");
        }

        public void FindPeRatio()
        {

        }
    }
}
