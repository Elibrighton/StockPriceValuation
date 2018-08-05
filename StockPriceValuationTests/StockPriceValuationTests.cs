using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPriceValuation.Services;

namespace StockPriceValuationTests
{
    [TestClass]
    public class StockPriceValuationTests
    {
        private Selenium _testSelenium;
        private string _testUrl;

        [TestInitialize]
        public void Initialise()
        {
            _testUrl = "https://quickfs.net/company/GOOG";
            _testSelenium = new Selenium(_testUrl);
        }

        [TestMethod]
        public void SeleniumGetElementTest()
        {
            var testTagName = "div";
            var testInnerText = "Overview";

            var element = _testSelenium.GetElementByText(testTagName, testInnerText);

            Assert.IsNotNull(element);
        }
    }
}
