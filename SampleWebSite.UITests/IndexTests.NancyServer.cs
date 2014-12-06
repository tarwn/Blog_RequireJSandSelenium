using Nancy.Hosting.Self;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using Owin;
using SampleWebSite.UITests.NancyServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SampleWebSite.UITests
{

    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(PhantomJSDriver))]
    public class IndexTests_NancyServer<TDriver>
        where TDriver : IWebDriver, new()
    {

        private string _baseUrl = "http://localhost:5000";
        private NancyHost _webServer;
        private IWebDriver _webDriver;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _webServer = SetupServer();
            _webDriver = new TDriver();
            _webDriver.Manage().Window.Size = new System.Drawing.Size(1280, 1024);
        }

        private NancyHost SetupServer()
        {
            var dnfo = new DirectoryInfo("TestSampleWebSite");
            if (dnfo.Exists)
                dnfo.Delete(true);

            var proc = new Process();
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.FileName = @"C:\WINDOWS\system32\xcopy.exe";
            proc.StartInfo.Arguments = "\"../../../SampleWebSite\" TestSampleWebSite /E /I";
            proc.Start();
            proc.WaitForExit();

            var config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations()
                {
                    User = "Everyone",
                    CreateAutomatically = true
                }
            };

            var host = new NancyHost(new LocalServerBootstrapper(dnfo.FullName), config, new Uri(_baseUrl));
            host.Start();
            return host;
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _webDriver.Quit();
            _webServer.Stop();
        }

        [TearDown]
        public void TestTeardown()
        {
            var dnfo = new DirectoryInfo("screenshots");
            if (!dnfo.Exists)
                dnfo.Create();

            string typeName = this.GetType().Name.Replace("`1","");
            string driverName = _webDriver.GetType().Name;
            string filename = String.Format("{0}/{1}___{2}___{3}.png",
                                            dnfo.FullName,
                                            typeName,
                                            TestContext.CurrentContext.Test.Name,
                                            driverName);

            if (typeof(ITakesScreenshot).IsAssignableFrom(_webDriver.GetType()))
            {
                ((ITakesScreenshot)_webDriver).GetScreenshot()
                                              .SaveAsFile(filename, ImageFormat.Png);

            }
        }

        [Test]
        public void WhenUserSearchesForItemsAndSelectsOne_ThenDetailsAreDisplayedForTheSelectedProduct()
        {
            var indexPage = new IndexPage(_webDriver, _baseUrl + "/index.html", "Sample App");

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
            var indexPage = new IndexPage(_webDriver, _baseUrl + "/index.html", "Sample App");

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
            var indexPage = new IndexPage(_webDriver, _baseUrl + "/index.html", "Sample App");

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
