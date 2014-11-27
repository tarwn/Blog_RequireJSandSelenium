using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebSite.UITests
{
    [TestFixture]
    public class IndexTests_FullIntegration
    {

        private IWebDriver _webDriver;
        private string _url = "http://localhost:63431/";

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _webDriver = new ChromeDriver();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _webDriver.Close();
            _webDriver.Dispose();
        }

        [Test]
        public void WhenUserSearchesForItemsAndSelectsOne_ThenDetailsAreDisplayedForTheSelectedProduct()
        {
            var indexPage = new IndexPage(_webDriver, _url, "Sample App");
            indexPage.SearchButton.Click();
            Utility.WaitUpTo(5000, () => Utility.IsElementPresent(indexPage.SearchResultsTable) && indexPage.SearchResultsTable.Displayed, "Search results");
            Assert.AreNotEqual(0, indexPage.GetNumberOfSearchResults());

            indexPage.ClickSearchResults(0);
            Utility.WaitUpTo(5000, () => Utility.IsElementPresent(indexPage.ItemDetails) && indexPage.ItemDetails.Displayed, "Item Details");

            Assert.AreEqual(indexPage.GetSelectedRowItemName(), indexPage.ItemDetailsName.Text);
        }
    }
}
