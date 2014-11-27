using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebSite.UITests
{
    public class IndexPage
    {
        private IWebDriver _webDriver;

        public IndexPage(IWebDriver webDriver, string url, string expectedTitle)
        {
            this._webDriver = webDriver;
            this._webDriver.Navigate().GoToUrl(url);

            Utility.WaitUpTo(5000, () => this._webDriver.FindElement(By.TagName("body")) != null, "waiting for body");

            Assert.AreEqual(expectedTitle, _webDriver.Title);

            PageFactory.InitElements(webDriver, this);
        }

        [FindsBy(How = How.Id, Using = "search-button")]
        public IWebElement SearchButton { get; set; }

        [FindsBy(How = How.CssSelector, Using = "table.search-results-items")]
        public IWebElement SearchResultsTable { get; set; }

        [FindsBy(How = How.Id, Using = "item-details")]
        public IWebElement ItemDetails { get; set; }

        [FindsBy(How = How.Id, Using = "item-details-name")]
        public IWebElement ItemDetailsName { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".selected-row")]
        public IWebElement SelectedSearchResultRow { get; set; }


        public void ClickSearchResults(int index) {
            var tbody = SearchResultsTable.FindElement(By.TagName("tbody"));
            var rows = tbody.FindElements(By.TagName("tr"));
            if (rows.Count > index) {
                rows[index].Click();
            }
        }

        public int GetNumberOfSearchResults()
        {
            var rows = SearchResultsTable.FindElement(By.TagName("tbody"))
                                         .FindElements(By.TagName("tr"));
            return rows.Count;
        }

        public string GetSelectedRowItemName() {
            var nameCell = SelectedSearchResultRow.FindElements(By.TagName("td"))[1];
            return nameCell.Text;
        }
    }
}
