using System;
using System.IO;
using System.Net;

namespace StockPriceValuation.Services
{
    public static class Web
    {
        public static string GetWebResponse(string url)
        {
            var attempts = 3;
            var responseFromServer = string.Empty;

            while (attempts > 0)
            {
                try
                {
                    var request = WebRequest.Create(url);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    var response = request.GetResponse();
                    var dataStream = response.GetResponseStream();
                    var reader = new StreamReader(dataStream);
                    responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    attempts = 0;
                }
                catch (Exception)
                {
                    attempts--;
                }
            }

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
