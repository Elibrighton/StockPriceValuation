using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceValuation.Services
{
    public class Selenium
    {
        const string _chromeDriverPath = @"C:\Users\st3gs\source\repos\StockPriceValuation\ExternalReference";

        private string _url;

        public ChromeDriver DriverChrome { get; set; }

        public Selenium(string url)
        {
            _url = url;
            DriverChrome = new ChromeDriver(_chromeDriverPath);
        }

        public IWebElement GetElementByText(string tagName, string innerText)
        {
            DriverChrome.Navigate().GoToUrl(_url);
            var elements = DriverChrome.FindElements(By.TagName(tagName));

            foreach (var element in elements)
            {
                if (element.Text == innerText)
                {
                    return element;
                }
            }

            return null;
        }

        public IWebElement GetParent(IWebElement element)
        {
            return element.FindElement(By.XPath(".."));
        }

        public string GetPageSource()
        {
            return DriverChrome.PageSource;
        }
    }
}
