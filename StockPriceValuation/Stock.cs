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

        }

        public void FindPeRatio()
        {

        }
    }
}
