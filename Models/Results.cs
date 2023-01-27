using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darktide_Armoury_Monitor
{
    public class Results
    {

        public ScraperArguments args;

        public int numTotalChecks;
        public int numTotalChecksWithHits;
        public int numTotalHits;
        public int numLastHits;
        public string hashHits;
        public bool notificationPushed;

        public string fatalError;
        public string errorLogSnag;
        public string nextRunTime;

        public Results(ScraperArguments args, 
            int numTotalChecks, int numTotalChecksWithHits, int numTotalHits, int numLastHits,
            bool notificationPushed, string nextRunTime, string fatalError, string errorLogSnag)
        {
            this.numTotalChecks = numTotalChecks;
            this.numTotalChecksWithHits = numTotalChecksWithHits;
            this.numTotalHits = numTotalHits;
            this.numLastHits = numLastHits;
            this.hashHits = "";
            this.notificationPushed = notificationPushed;
            this.nextRunTime = nextRunTime;

            this.fatalError = fatalError;
            this.errorLogSnag = errorLogSnag;
        }

    }

}
