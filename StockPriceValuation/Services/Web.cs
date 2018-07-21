using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Services
{
    public static class Web
    {
        public static string GetWebResponse(string url)
        {
            var request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return responseFromServer;
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

        public static Stream GetStream(string url)
        {
            var stream = new MemoryStream();

            var webResponse = Web.GetWebResponse(url);

            if (!string.IsNullOrEmpty(webResponse))
            {
                return GenerateStreamFromString(webResponse);
            }

            return stream;
        }
    }
}
