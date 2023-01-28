using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Darktide_Armoury_Monitor
{
    public partial class MainForm : Form
    {
        BackgroundWorker bw;
        MainConfig config;
        XPathConfig xpathConfig;

        public MainForm()
        {
            InitializeComponent();

            config = new MainConfig();
            xpathConfig = new XPathConfig();
            SaveNewConfigs();

            bw = new BackgroundWorker();

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;

            bw.DoWork += new DoWorkEventHandler(Scraper.bw_entry);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkComplete);
            bw.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);

        }

        private void SaveNewConfigs()
        {
            if(!File.Exists("config.xml")) {
                config.SaveConfig();
            }

            if(!File.Exists("config_xpath.xml")) {
                xpathConfig.SaveConfig();
            }

        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if(e.UserState != null) {
                Results results = (Results)e.UserState;
                lbl_nextRunTime.Visible = true;

                //***1) Add an entry to the recent history
                string curTime = DateTime.Now.ToString("HH:mm:ss");
                string curResult = curTime + " : number of matches: " + results.numLastHits;

                if(results.errorLogSnag != "") {
                    curResult += " Error: " + results.errorLogSnag;
                }

                if(results.notificationPushed) {
                    curResult += " (notification attempt pushed)";
                }

                List<string> logResults = txt_results.Text.Split('\n').ToList();

                if(logResults.Count > 8) {
                    logResults.RemoveAt(0);
                }

                logResults.Add(curResult);

                StringBuilder sb = new StringBuilder(160);
                for(int i = 0; i < logResults.Count; i++) {
                    if(logResults[i] == "") continue;

                    sb.Append(logResults[i] + Environment.NewLine);
                }



                txt_results.Text = sb.ToString();

                //***2) Update labels for historical totals
                lbl_totalChecks.Text = results.numTotalChecks.ToString();
                lbl_totalChecksWithHits.Text = results.numTotalChecksWithHits.ToString();
                lbl_totalHits.Text = results.numTotalHits.ToString();

                lbl_nextRunTime.Text = results.nextRunTime;

            }
        }

        private void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            btn_start.Enabled = true;
            btn_stop.Enabled = false;

            Results results = (Results)e.Result;

            if(results.fatalError != "") {
                MessageBox.Show(results.fatalError, "Error:");
            }

        }

        private bool ValidateArgument(ScraperArguments args, out string errorMsg)
        {
            bool valid = true;
            errorMsg = "";

            string scrapingProfilePathL = args.scrapingProfilePath.ToLower();

            if(scrapingProfilePathL == "" || scrapingProfilePathL.Contains("replace this")) {
                valid = false;
                errorMsg += "Please edit config.xml to and provide a path for the scraping browser profile" + Environment.NewLine;
            }
            else if(!Directory.Exists(scrapingProfilePathL)) {
                valid = false;
                errorMsg += "Scraping browser profile path is not accessible at specified location:" + 
                    args.scrapingProfilePath + Environment.NewLine;
            }
            else if(!args.qcMode && scrapingProfilePathL.Contains("chrome")) {
                valid = false;
                errorMsg += "Scraping with chrome in headless mode is currently not supported. Please use a Firefox profile for scraping.";
            }

            if(args.resultsProfilePath == "" || args.resultsProfilePath.Contains("Replace this")) {
                valid = false;
                errorMsg += "Please provide a path for the results browser profile" + Environment.NewLine;
            }
            else if(!Directory.Exists(args.resultsProfilePath)) {
                valid = false;
                errorMsg += "Results browser profile path is not accessible at specified location:" + 
                    args.scrapingProfilePath + Environment.NewLine;
            }

            if(args.scrapingProfilePath != "" && args.scrapingProfilePath == args.resultsProfilePath) {
                valid = false;
                errorMsg += "Please do not use the same browser profile for both scraping and launching results";
            }

            if(args.pushToLogSnag) {

                if(args.logSnagAPIToken == "" || args.logSnagAPIToken.Contains("Replace this")) {
                    valid = false;
                    errorMsg += "Please provide the LogSnag API Token" + Environment.NewLine;
                }

                if(args.logSnagProject == "" || args.logSnagProject.Contains("Replace this")) {
                    valid = false;
                    errorMsg += "Please provide the LogSnag Channel" + Environment.NewLine;
                }

                if(args.logSnagChannel == "" || args.logSnagChannel.Contains("Replace this")) {
                    valid = false;
                    errorMsg += "Please provide the LogSnag Channel" + Environment.NewLine;
                }
            }



            return valid;

        }

        private void btn_start_Click(object sender, EventArgs e)
        {

            if(!bw.IsBusy) {

                SaveNewConfigs();
                config.LoadConfig();
                xpathConfig.LoadConfig();

                ScraperArguments args = new ScraperArguments() {
                    qcMode = chk_qcMode.Checked,
                    pushToLogSnag = chk_pushToLogSnag.Checked,
                    scrapingProfilePath = config.scrapingProfilePath,
                    resultsProfilePath = config.resultsProfilePath,
                    logSnagAPIToken = config.logSnagAPIToken,
                    logSnagChannel = config.logSnagChannel,
                    logSnagProject = config.logSnagProject,
                    xpathConfig = xpathConfig
                };

                string errorMsg;
                if(!ValidateArgument(args, out errorMsg)) {
                    MessageBox.Show(errorMsg, "Argument Errors: (Check XML config)");
                    return;
                }

                btn_start.Enabled = false;
                btn_stop.Enabled = true;

                bw.RunWorkerAsync(args);
            }
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            if(bw.IsBusy) {
                bw.CancelAsync();
                Scraper.StopRunning();
            }           
            
        }
    }
}
