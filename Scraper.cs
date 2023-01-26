﻿using System;
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
        private static string errorMsg;

        private static Dictionary<string, bool> hashesForCreditHitsTable;
        private static Dictionary<string, bool> hashesForMarkHitsTable;

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

        }

        private static void Initialize()
        {
            continueRunning = true;
            numTotalChecks = 0;
            numTotalChecksWithHits = 0;
            numTotalHits = 0;
            numLastHits = 0;
            errorMsg = "";

            scrapingWithFirefox = false;

            if(args.scrapingProfilePath.ToLower().Contains("firefox")) {
                scrapingWithFirefox = true;
            }

            if(args.resultsProfilePath.ToLower().Contains("firefox")) {
                resultsWithFirefox = true;
            }

            hashesForCreditHitsTable = new Dictionary<string, bool>();
            hashesForMarkHitsTable = new Dictionary<string, bool>();
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
            IWebDriver driver;            

            if(scrapingWithFirefox) {
                driver = InitializeDriverFirefox(url);
            }
            else {
                driver = InitializeDriverChrome(url);
            }
            
             
            if(driver == null) return;
            bool notificationSent = false;
            int msUntilNextRun = -1;

            while(continueRunning) {
                errorMsg = "";
                List<ItemOffer> allCreditMatches;
                List<ItemOffer> allMarksMatches;
                notificationSent = false;

                if (CheckSiteForMatches(driver, forceRefresh: numTotalChecks>0,
                    out numLastHits, out allCreditMatches, out allMarksMatches, out msUntilNextRun)) {

                    //***Now do some work to determine if we have new results or not
                    bool newHits = false;

                    if(allCreditMatches.Count > 0) {
                        string creditsResultsHash = GetHashFromMatches(allCreditMatches);
                        if(!hashesForCreditHitsTable.ContainsKey(creditsResultsHash)) {
                            newHits = true;

                            StringBuilder sbCredits = new StringBuilder(40);
                            for(int i = 0; i < allCreditMatches.Count; i++) {
                                sbCredits.Append(allCreditMatches[i].name);

                                if(i < allCreditMatches.Count - 1) {
                                    sbCredits.Append("; ");
                                }
                            }

                            if(args.pushToLogSnag) {
                                Notifier.SendNotification(args.logSnagAPIToken, args.logSnagProject,
                                    args.logSnagChannel, "Matches Found !(Credits)", sbCredits.ToString(),
                                    out errorMsg);
                                notificationSent = true;
                            }

                        }

                    }
                    else {
                        hashesForCreditHitsTable.Clear();
                    }

                    if(allMarksMatches.Count > 0) {
                        string marksResultsHash = GetHashFromMatches(allMarksMatches);
                        if(!hashesForMarkHitsTable.ContainsKey(marksResultsHash)) {
                            newHits = true;

                            StringBuilder sbMarks = new StringBuilder(40);
                            for(int i = 0; i < allMarksMatches.Count; i++) {
                                sbMarks.Append(allMarksMatches[i].name);

                                if(i < allMarksMatches.Count - 1) {
                                    sbMarks.Append("; ");
                                }
                            }

                            if(args.pushToLogSnag) {
                                if (allCreditMatches.Count > 0) Thread.Sleep(5000);

                                Notifier.SendNotification(args.logSnagAPIToken, args.logSnagProject,
                                    args.logSnagChannel, "Matches Found! (Marks)", sbMarks.ToString(),
                                    out errorMsg);
                                notificationSent = true;
                            }

                        }

                    }
                    else {
                        hashesForMarkHitsTable.Clear();
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
            
            Shutdown(driver);

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
                if(SiteElementChecks.AtPlatformSignIn(driver, out matchedElem)) {
                    matchedElem.Click();
                    Thread.Sleep(3000);
                }

                //***Are we at steam account selection?
                if(SiteElementChecks.AtSteamSignIn(driver, out matchedElem)) {
                    matchedElem.Click();
                    Thread.Sleep(3000);
                }

                Thread.Sleep(6500); //wait for armoury extension to load/display items

                //TODO: do some safe checks to see if we're at the right section now

                //***Assuming that we're now at the desired results page...
                IWebElement storeTypeMenu = driver.FindElement(By.XPath("//select[@id='store_type_0']"));
                var selectElem = new SelectElement(storeTypeMenu);
                
                //***Do a first run on the credit shop
                selectElem.SelectByValue("credits");
                Thread.Sleep(2000);
                List<IWebElement> characterButtons = SiteElementChecks.GetCharacterButtons(driver);
                for(int i = 0; i < characterButtons.Count; i++) {
                    IWebElement curCharButton = characterButtons[i];
                    curCharButton.Click();
                    Thread.Sleep(1500);

                    List<IWebElement> curCharMatches;
                    if(SiteElementChecks.HasMatchingOffers(driver, out curCharMatches)) {
                        matchesFound = true;
                        numHits++;
                        numTotalHits++;

                        for(int j = 0; j < curCharMatches.Count; j++) {
                            matchesByCredits.Add( ParseMatch(curCharMatches[j] ));
                        }

                    }

                }

                msUntilCreditsRefresh = SiteElementChecks.GetMSUntilRefresh(driver);

                //***Do a second run on the marks shop
                selectElem.SelectByValue("marks");
                Thread.Sleep(2000);
                for(int i = 0; i < characterButtons.Count; i++) {
                    IWebElement curCharButton = characterButtons[i];
                    curCharButton.Click();
                    Thread.Sleep(1500);

                    List<IWebElement> curCharMatches;
                    if(SiteElementChecks.HasMatchingOffers(driver, out curCharMatches)) {
                        matchesFound = true;
                        numHits++;
                        numTotalHits++;

                        for(int j = 0; j < curCharMatches.Count; j++) {
                            matchesByMarks.Add( ParseMatch(curCharMatches[j] ));
                        }

                    }

                }
                
                msUntilMarksRefresh = SiteElementChecks.GetMSUntilRefresh(driver);

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
            ;
            DateTime dt = DateTime.Now;
            dt = dt.AddMilliseconds(msUntilNextRun);
            string nextRunTime = dt.ToString("HH:mm");

            Results results = new Results(args, numTotalChecks, numTotalChecksWithHits, 
                numTotalHits, numLastHits, notificationPushed, nextRunTime, errorMsg);

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

            SoundPlayer player = new SoundPlayer(Darktide_Armoury_Monitor.Properties.Resources.chimes);
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

            SoundPlayer player = new SoundPlayer(Darktide_Armoury_Monitor.Properties.Resources.chimes);
            player.Play();

        }

        private static IWebDriver InitializeDriverChrome(string url)
        {
            try {
                IWebDriver driver = null;
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("user-data-dir=C:\\Users\\Dan\\AppData\\Local\\Google\\Chrome\\User Data");
                //options.AddArgument("--no-sandbox");
                //options.AddArgument("--disable-dev-shm-usage");


                string path = @"G:\Programming\Web_scraping\Darktide_Armoury_Monitor\bin\Debug\chromedriver.exe";

                driver = new ChromeDriver(path, options);

                driver.Url = url;     
                
                return driver;
            }
            catch (Exception err) {
                //TODO: take action
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