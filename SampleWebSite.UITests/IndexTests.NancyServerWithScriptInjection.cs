﻿using Nancy.Hosting.Self;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using Owin;
using SampleWebSite.UITests.NancyServerWithScriptInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebSite.UITests
{

    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(PhantomJSDriver))]
    public class IndexTests_NancyServerWithScriptInjection<TDriver>
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
