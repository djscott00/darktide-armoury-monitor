using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darktide_Armoury_Monitor
{
    public class ScraperArguments
    {

        public bool qcMode;
        public bool pushToLogSnag;
        public string scrapingProfilePath;
        public string resultsProfilePath;
        public string logSnagProject;
        public string logSnagChannel;
        public string logSnagAPIToken;

        public XPathConfig xpathConfig;

        public ScraperArguments()
        {
            qcMode = false;
            pushToLogSnag = false;
            scrapingProfilePath = "";
            resultsProfilePath = "";
            logSnagProject ="";
            logSnagChannel = "";
            logSnagAPIToken = "";
            xpathConfig = null;
        }


    }


}
