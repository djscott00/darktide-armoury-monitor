using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace Darktide_Armoury_Monitor
{
    [DataContract]
    public class MainConfig
    {

        [DataMember]
        public string scrapingProfilePath;

        [DataMember]
        public string resultsProfilePath;

        [DataMember]
        public string logSnagProject;
        [DataMember]
        public string logSnagChannel;
        [DataMember]
        public string logSnagAPIToken;

        public MainConfig()
        {
            scrapingProfilePath = "Replace this with a path to a Firefox profile or Chrome profile. Do not use your primary/default profile.";
            resultsProfilePath = "Replace this with a path to a Firefox profile or Chrome profile";
            logSnagProject = "Replace this (make a Log Snag account and project)";
            logSnagChannel = "Replace this (make a Log Snag account, project, and a channel)";
            logSnagAPIToken = "Replace this (make a Log Snag account, find your API token)";
        }

        public void SaveConfig()
        {
            DataContractSerializer dcs = new DataContractSerializer( this.GetType() );
            XmlWriterSettings settings = new XmlWriterSettings {Indent = true };

            using (XmlWriter writer = XmlWriter.Create("config.xml", settings)) {
                dcs.WriteObject(writer, this);
            }
        }

        public bool LoadConfig()
        {
            if(!File.Exists("config.xml")) {
                return false;
            }

            DataContractSerializer dcs = new DataContractSerializer( this.GetType() );

            using(XmlReader reader = XmlReader.Create("config.xml")) {
                MainConfig readConfig = (MainConfig)dcs.ReadObject(reader);

                this.scrapingProfilePath = readConfig.scrapingProfilePath;
                this.resultsProfilePath = readConfig.resultsProfilePath;
                this.logSnagProject = readConfig.logSnagProject;
                this.logSnagChannel = readConfig.logSnagChannel;
                this.logSnagAPIToken = readConfig.logSnagAPIToken;  
            }
            
            return true;

        }


    }


}
