using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebSite.UITests
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(PhantomJSDriver))]
    public class IndexTests_FullIntegration<TDriver>
        where TDriver : IWebDriver, new()
    {

        private IWebDriver _webDriver;
        private string _url = "http://localhost:63431/";

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _webDriver = new TDriver();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _webDriver.Quit();
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

        [Test]
        public void WhenUserSearchesForItemsAndAddsOneToCart_ThenTheItemAppearsInTheCart()
        {
            var indexPage = new IndexPage(_webDriver, _url, "Sample App");

            indexPage.SearchButton.Click();
            Utility.WaitUpTo(5000, () => Utility.IsElementPresent(indexPage.SearchResultsTable) && indexPage.SearchResultsTable.Displayed, "Search results");
            Assert.AreNotEqual(0, indexPage.GetNumberOfSearchResults());

            indexPage.ClickSearchResultAddButtonForRow(0);
            indexPage.LookAtCartDetails();
            Assert.AreEqual("1", indexPage.CartCount.Text);
            Assert.AreEqual(1, indexPage.GetNumberOfCartDetailsRows());
        }

        [Test]
        public void WhenUserSearchesForItemsAndClicksOneToViewDetailsAndAddsToCart_ThenTheItemAppearsInTheCart()
        {
            var indexPage = new IndexPage(_webDriver, _url, "Sample App");

            indexPage.SearchButton.Click();
            Utility.WaitUpTo(5000, () => Utility.IsElementPresent(indexPage.SearchResultsTable) && indexPage.SearchResultsTable.Displayed, "Search results");
            Assert.AreNotEqual(0, indexPage.GetNumberOfSearchResults());

            indexPage.ClickSearchResults(0);
            Utility.WaitUpTo(5000, () => Utility.IsElementPresent(indexPage.ItemDetails) && indexPage.ItemDetails.Displayed, "Item Details");

            indexPage.ItemDetailsAddToCart.Click();
            indexPage.LookAtCartDetails();
            Assert.AreEqual("1", indexPage.CartCount.Text);
            Assert.AreEqual(1, indexPage.GetNumberOfCartDetailsRows());
        }
    }
}
