using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Darktide_Armoury_Monitor
{
    public static class Scraper
    {

        private static readonly object locker;

        private static bool continueRunning;
        private const int fallbackDelayMS = 1200*1000;

        private static BackgroundWorker bw;
        private static ScraperArguments args; //placeholder for input arguments

        private static Thread scrapingThread;

        private static int numTotalChecks;
        private static int numTotalChecksWithHits;
        private static int numTotalHits;
        private static int numLastHits;
        private static string logSnagErrorMsg;
        private static string fatalErrorMsg;

        private static string hashLastCreditsMatch;
        private static string hashLastMarksMatch;

        private static bool scrapingWithFirefox;
        private static bool resultsWithFirefox;

        static Scraper()
        {
            locker = new object();
        }

        public static void bw_entry(object sender, DoWorkEventArgs e)
        {
            bw = (BackgroundWorker)sender;
            args = (ScraperArguments)e.Argument;

            Initialize();

            string mainUrl = "https://accounts.atoma.cloud/dashboard";

            ParameterizedThreadStart starter = new ParameterizedThreadStart(RepeatChecks);
            scrapingThread = new Thread(starter);
            scrapingThread.Start(mainUrl);

            scrapingThread.Join();

            Results results = new Results(args, numTotalChecks, numTotalChecksWithHits, 
                numTotalHits, numLastHits, false, "", fatalErrorMsg, logSnagErrorMsg);

            e.Result = results;

        }

        private static void Initialize()
        {
            continueRunning = true;
            numTotalChecks = 0;
            numTotalChecksWithHits = 0;
            numTotalHits = 0;
            numLastHits = 0;
            logSnagErrorMsg = "";
            fatalErrorMsg = "";

            scrapingWithFirefox = false;

            if(args.scrapingProfilePath.ToLower().Contains("firefox")) {
                scrapingWithFirefox = true;
            }

            if(args.resultsProfilePath.ToLower().Contains("firefox")) {
                resultsWithFirefox = true;
            }

            hashLastCreditsMatch = "";
            hashLastMarksMatch = "";

        }

        public static void StopRunning()
        {
            if(continueRunning) {
                continueRunning = false;

                if(scrapingThread != null) {
                    scrapingThread.Interrupt();
                }

            }

        }

        //***The Main repeating loop
        private static void RepeatChecks(object urlObj)
        {
            string url = (string)urlObj;
            IWebDriver driver = null;;   

            try {

                if(scrapingWithFirefox) {
                    driver = InitializeDriverFirefox(url);
                }
                else {
                    driver = InitializeDriverChrome(url);
                }            
             
                bool notificationSent = false;
                int msUntilNextRun = -1;

                while(continueRunning) {

                    logSnagErrorMsg = "";
                    List<ItemOffer> allCreditMatches;
                    List<ItemOffer> allMarksMatches;
                    notificationSent = false;

                    if (CheckSiteForMatches(driver, forceRefresh: numTotalChecks>0,
                        out numLastHits, out allCreditMatches, out allMarksMatches, out msUntilNextRun)) {

                        //***Now do some work to determine if we have new results or not
                        bool newHits = false;

                        if(allCreditMatches.Count > 0) {
                            string creditsResultsHash = GetHashFromMatches(allCreditMatches);

                            if(creditsResultsHash != "" && creditsResultsHash != hashLastCreditsMatch) {
                                newHits = true;
                                hashLastCreditsMatch = creditsResultsHash;

                                if(args.pushToLogSnag) {
                                    StringBuilder sbCredits = new StringBuilder(40);
                                    for(int i = 0; i < allCreditMatches.Count; i++) {
                                        sbCredits.Append(allCreditMatches[i].name);

                                        if(i < allCreditMatches.Count - 1) {
                                            sbCredits.Append("; ");
                                        }
                                    }

                                    Notifier.SendNotification(args.logSnagAPIToken, args.logSnagProject,
                                        args.logSnagChannel, "Matches Found! (Credits)", sbCredits.ToString(),
                                        out logSnagErrorMsg);
                                    notificationSent = true;
                                }

                            }

                        }

                        if(allMarksMatches.Count > 0) {
                            string marksResultsHash = GetHashFromMatches(allMarksMatches);

                            if(marksResultsHash != "" && marksResultsHash != hashLastMarksMatch) {
                                newHits = true;
                                hashLastMarksMatch = marksResultsHash;

                                if(args.pushToLogSnag) {
                                    StringBuilder sbMarks = new StringBuilder(40);
                                    for(int i = 0; i < allMarksMatches.Count; i++) {
                                        sbMarks.Append(allMarksMatches[i].name);

                                        if(i < allMarksMatches.Count - 1) {
                                            sbMarks.Append("; ");
                                        }
                                    }

                                    if (allCreditMatches.Count > 0) Thread.Sleep(5000);

                                    Notifier.SendNotification(args.logSnagAPIToken, args.logSnagProject,
                                        args.logSnagChannel, "Matches Found! (Marks)", sbMarks.ToString(),
                                        out logSnagErrorMsg);
                                    notificationSent = true;
                                }

                            }

                        }


                        if(newHits) {
                            if(resultsWithFirefox) {
                                LaunchResultsFirefox(url, args.resultsProfilePath);
                            }
                            else {
                                LaunchResultsChrome(url, args.resultsProfilePath);
                            }
                        
                            numTotalChecksWithHits++;
                        }
                    

                    }

                    numTotalChecks++;
                    ReportProgress(notificationSent, msUntilNextRun >= 0 ? msUntilNextRun: fallbackDelayMS);

                    try {

                        if(msUntilNextRun >= 0) {
                            Thread.Sleep(msUntilNextRun);
                        }
                        else {
                            Thread.Sleep(fallbackDelayMS);
                        }

                    
                    }
                    catch (ThreadInterruptedException ex) {

                    }
                    catch (ThreadAbortException ex) {

                    }

                }
            
            }
            catch (Exception err) {

                bool screenshotSaved = false;
                if(driver != null) {
                    Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                    ss.SaveAsFile("screenshotException.png", ScreenshotImageFormat.Png);
                    screenshotSaved = true;
                }

                fatalErrorMsg = err.Message;

                if(screenshotSaved) {
                    fatalErrorMsg += Environment.NewLine + "(saved view of scraper to screenshotException.png)";
                }

            }
            finally {
                Shutdown(driver);
            }

        }


        //***The Main set of checks for a run
        private static bool CheckSiteForMatches(IWebDriver driver, bool forceRefresh, 
            out int numHits, 
            out List<ItemOffer> matchesByCredits, 
            out List<ItemOffer> matchesByMarks,
            out int msUntilNextRun)
        {
            IWebElement matchedElem;
            bool matchesFound = false;
            numHits = 0;
            matchesByCredits = new List<ItemOffer>();
            matchesByMarks = new List<ItemOffer>();

            int msUntilCreditsRefresh = -1;
            int msUntilMarksRefresh = -1;
            msUntilNextRun = -1;
            
            try {

                if(forceRefresh) {
                    driver.Navigate().Refresh();
                }

                //***Are we at initial login page?
                if(SiteElementChecks.AtPlatformSignIn(args.xpathConfig, driver, out matchedElem)) {
                    matchedElem.Click();
                    Thread.Sleep(3000);
                }

                //***Are we at steam account selection?
                if(SiteElementChecks.AtSteamSignIn(args.xpathConfig, driver, out matchedElem)) {
                    matchedElem.Click();
                    Thread.Sleep(3000);
                }

                Thread.Sleep(6500); //wait for armoury extension to load/display items

                //***Assuming that we're now at the desired results page...
                IWebElement storeTypeMenu = driver.FindElement(By.XPath(args.xpathConfig.storeTypeDropdown));
                var selectElem = new SelectElement(storeTypeMenu);
                
                //***Do a first run on the credit shop
                selectElem.SelectByValue(args.xpathConfig.storeOptionCredits);
                Thread.Sleep(2000);
                List<IWebElement> characterButtons = SiteElementChecks.GetCharacterButtons(args.xpathConfig, driver);
                for(int i = 0; i < characterButtons.Count; i++) {
                    IWebElement curCharButton = characterButtons[i];
                    curCharButton.Click();
                    Thread.Sleep(1500);

                    List<IWebElement> curCharMatches;
                    if(SiteElementChecks.HasMatchingOffers(args.xpathConfig, driver, out curCharMatches)) {
                        matchesFound = true;
                        numHits++;
                        numTotalHits++;

                        for(int j = 0; j < curCharMatches.Count; j++) {
                            matchesByCredits.Add( ParseMatch(curCharMatches[j] ));
                        }

                    }

                }

                msUntilCreditsRefresh = SiteElementChecks.GetMSUntilRefresh(args.xpathConfig, driver);

                //***Do a second run on the marks shop
                selectElem.SelectByValue(args.xpathConfig.storeOptionMarks);
                Thread.Sleep(2000);
                for(int i = 0; i < characterButtons.Count; i++) {
                    IWebElement curCharButton = characterButtons[i];
                    curCharButton.Click();
                    Thread.Sleep(1500);

                    List<IWebElement> curCharMatches;
                    if(SiteElementChecks.HasMatchingOffers(args.xpathConfig, driver, out curCharMatches)) {
                        matchesFound = true;
                        numHits++;
                        numTotalHits++;

                        for(int j = 0; j < curCharMatches.Count; j++) {
                            matchesByMarks.Add( ParseMatch(curCharMatches[j] ));
                        }

                    }

                }
                
                msUntilMarksRefresh = SiteElementChecks.GetMSUntilRefresh(args.xpathConfig, driver);

            }
            catch (ThreadInterruptedException ex) {

            }
            catch (ThreadAbortException ex) {

            }

            if(msUntilCreditsRefresh >= 0 && msUntilMarksRefresh >= 0) {
                if(msUntilCreditsRefresh < msUntilMarksRefresh) {
                    msUntilNextRun = msUntilCreditsRefresh;
                }
                else {
                    msUntilNextRun = msUntilMarksRefresh;
                }
            }

            return matchesFound;
        }

        private static void ReportProgress(bool notificationPushed, int msUntilNextRun)
        {
            DateTime dt = DateTime.Now;
            dt = dt.AddMilliseconds(msUntilNextRun);
            string nextRunTime = dt.ToString("HH:mm");

            Results results = new Results(args, numTotalChecks, numTotalChecksWithHits, 
                numTotalHits, numLastHits, notificationPushed, nextRunTime, "", logSnagErrorMsg);

            bw.ReportProgress(0, results);

        }

        private static void Shutdown(IWebDriver driver)
        {
            if (driver != null) {
                driver.Quit();
                //driver.Close();
            }
        }


        private static void LaunchResultsFirefox(string url, string profileFolderPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "firefox.exe";
            process.StartInfo.Arguments = url + 
                $" -profile \"{profileFolderPath}\"";
            process.Start();

            SoundPlayer player = new SoundPlayer(Properties.Resources.chimes);
            player.Play();

        }

        private static void LaunchResultsChrome(string url, string profileFolderPath)
        {
            int lastSlash = profileFolderPath.LastIndexOf("\\");
            string profileName = profileFolderPath.Substring(lastSlash+1);
            
            Process process = new Process();
            process.StartInfo.FileName = "chrome.exe";
            process.StartInfo.Arguments = url + 
                " --new-window" + 
                $" --profile-directory=\"{ profileName }\"";
            process.Start();

            SoundPlayer player = new SoundPlayer(Properties.Resources.chimes);
            player.Play();

        }

        private static IWebDriver InitializeDriverChrome(string url)
        {
            try {
                int lastSlash = args.scrapingProfilePath.LastIndexOf('\\');
                string dataDir = args.scrapingProfilePath.Substring(0, lastSlash);
                string profileName = args.scrapingProfilePath.Substring(lastSlash +1);
                
                IWebDriver driver = null;
                ChromeOptions options = new ChromeOptions();
                options.AddArgument($"user-data-dir={dataDir}");
                options.AddArgument($"profile-directory={profileName}");

                if(!args.qcMode) {
                    options.AddArgument("headless");
                    options.AddArgument("--window-size=1920,1080");
                    options.AddArgument("--start-maximized");
                    options.AddArgument("--disable-gpu");
                    options.AddArgument("--disable-dev-shm-usage");
                    options.AddArgument("--no-sandbox");
                }
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("--allow-running-insecure-content");

                options.PageLoadStrategy = PageLoadStrategy.Normal;

                string path = "chromedriver.exe";

                driver = new ChromeDriver(path, options);

                driver.Url = url;     
                
                return driver;
            }
            catch (Exception err) {
                //TODO: take/report action
                ;
                return null;
            }
        }

        private static IWebDriver InitializeDriverFirefox(string url)
        {
            try {
                FirefoxOptions options = new FirefoxOptions();

                if(!args.qcMode) {
                    options.AddArgument("--headless");
                }
                options.PageLoadStrategy = PageLoadStrategy.Normal;

                string profilePath = args.scrapingProfilePath;
                FirefoxProfile profile = new FirefoxProfile(profilePath);

                options.Profile = profile;

                IWebDriver driver = new FirefoxDriver(options);
                driver.Url = url;

                return driver;
            }
            catch (Exception err) {
                return null;
            }
        }

        private static ItemOffer ParseMatch(IWebElement itemElem)
        {

            ItemOffer result = null;

            try {
                IWebElement titleElem = itemElem.FindElement(By.ClassName("item-title"));
                string title = titleElem.Text;
                int rating = 0;
                int credits = 0;
                int marks = 0;

                var infos = itemElem.FindElements(By.ClassName("info-item"));
                foreach(IWebElement info in infos) {
                    string curText = info.Text.ToLower();

                    if(curText.Contains("rating")) {
                        rating = GeneralMethods.ParseNumber(curText);
                    }
                    else if(curText.Contains("credits")) {
                        credits = GeneralMethods.ParseNumber(curText);
                    }
                    else if(curText.Contains("marks")) {
                        marks = GeneralMethods.ParseNumber(curText);
                    }
                }

                result = new ItemOffer(title, rating, credits, marks);
            }
            catch (Exception err) {

            }

            return result;

        }

        private static string GetHashFromMatches(List<ItemOffer> allMatchedOffers)
        {

            StringBuilder sb = new StringBuilder(40);
            for(int i = 0; i < allMatchedOffers.Count; i++) {
                sb.Append(allMatchedOffers[i].name);
                sb.Append(allMatchedOffers[i].rating);
                sb.Append(allMatchedOffers[i].credits);
                sb.Append(allMatchedOffers[i].marks);
            }

            string hashInput = sb.ToString();

            return GeneralMethods.GetHashString(hashInput);

        }


    }

}
