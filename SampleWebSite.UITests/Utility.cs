using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleWebSite.UITests
{
    public class Utility
    {
        public static void WaitUpTo(int milliseconds, Func<bool> Condition, string description)
        {
            int timeElapsed = 0;
            while (!Condition() && timeElapsed < milliseconds)
            {
                Thread.Sleep(100);
                timeElapsed += 100;
            }

            if (timeElapsed >= milliseconds || !Condition())
            {
                throw new AssertionException("Timed out while waiting for: " + description);
            }
        }

        public static bool IsElementPresent(IWebElement element)
        {
            try
            {
                bool b = element.Displayed;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
