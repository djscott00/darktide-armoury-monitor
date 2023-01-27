using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darktide_Armoury_Monitor
{
    public static class SiteElementChecks
    {


        public static bool AtPlatformSignIn(XPathConfig config, IWebDriver driver, out IWebElement matchedElem)
        {
            matchedElem = null;

            try {
                matchedElem = driver.FindElement(By.XPath(config.platformSignInButton));
                return true;
            }
            catch {
                return false;
            }

        }

        public static bool AtSteamSignIn(XPathConfig config, IWebDriver driver, out IWebElement matchedElem)
        {
            matchedElem = null;

            try {
                matchedElem = driver.FindElement(By.XPath(config.steamLoginButton));
                return true;
            }
            catch {
                return false;
            }

        }

        public static List<IWebElement> GetCharacterButtons(XPathConfig config, IWebDriver driver)
        {
            List<IWebElement> results = new List<IWebElement>();

            try {
                var temp = driver.FindElements(By.XPath(config.characterButtons));
                foreach(IWebElement match in temp) {
                    results.Add(match);
                }
            }
            catch {

            }

            return results;

        }

        public static bool HasMatchingOffers(XPathConfig config, IWebDriver driver, out List<IWebElement> allMatches)
        {

            allMatches = new List<IWebElement>();

            try {
                var temp = driver.FindElements(By.XPath(config.matchingOfferContainers));
                foreach (IWebElement potentialMatch in temp) {

                    string classes = potentialMatch.GetAttribute("class");
                    if(!classes.Contains(config.alreadyOwnedClass)) {
                        allMatches.Add(potentialMatch);
                    }

                }

                return allMatches.Count > 0;

            }
            catch {
                return false;
            }
        }

        public static int GetMSUntilRefresh(XPathConfig config, IWebDriver driver)
        {
            try {
                IWebElement refreshElem = driver.FindElement(By.XPath(config.refreshTimeDiv));
                string text = refreshElem.Text;

                int num = GeneralMethods.ParseNumber(text);

                if(text.Contains("hour")) {
                    return (num*60*60*1000) + 120000;
                }
                else if(text.Contains("minute")) {
                    return (num*60*1000) + 120000;
                }
                else if(text.Contains("second")) {
                    return (num*1000) + 120000;
                }


            }
            catch {

            }

            return -1;

        }



    }

}
