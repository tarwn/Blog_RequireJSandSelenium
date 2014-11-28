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
    public class IndexTests_ClientSideInjection
    {

        private IWebDriver _webDriver;
        private string _url = "http://localhost:63431/";

        private void InjectReplacementItemServiceMethods()
        {
            // the tricky part is that everything may have already instantiated the service, so we
            //  need to replace the prototype methods (if it was a singleton, we could replace those)
            //  and ensure the rest of our code uses the service instead of preserving references
            //  to direct methods anywhere

            if (!typeof(IJavaScriptExecutor).IsAssignableFrom(_webDriver.GetType()))
                throw new Exception("Using a WebDriver that does not support javascript execution. Bet that makes testing a SPA really tricky...");

            ((IJavaScriptExecutor)_webDriver).ExecuteScript(@"
                var fakeServiceResults = [
                    { id: 1, name: 'One', inStock: 5, price: 5.50, summary: 'product one' },
                    { id: 2, name: 'product two', inStock: 1, price: 6.25, summary: 'product two' },
                    { id: 3, name: 'product three', inStock: 2, price: 2.50, summary: 'product three' },
                    { id: 4, name: 'product four', inStock: 0, price: 7.00, summary: 'product four' }
                ];

                require(['app/services/itemService'], function(ItemService){

                    ItemService.prototype.search = function injectedSearch(searchText) {
                        var promise = $.Deferred();
                        promise.resolve(fakeServiceResults);
                        return promise;
                    };

                    ItemService.prototype.getItem = function injectedGetItem(itemId) { 
                        var promise = $.Deferred();
                        var matchingItem = _.find(this.fakeServiceResults, function (item) {
                            return item.id == itemId;
                        });
                        promise.resolve(matchingItem);
                        return promise;
                    };
                });
            ");
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _webDriver = new ChromeDriver();
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
            InjectReplacementItemServiceMethods();

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
            InjectReplacementItemServiceMethods();
            
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
            InjectReplacementItemServiceMethods();

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
