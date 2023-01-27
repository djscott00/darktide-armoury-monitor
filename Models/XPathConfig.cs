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
    public class XPathConfig
    {

        [DataMember]
        public string platformSignInButton;

        [DataMember]
        public string steamLoginButton;

        [DataMember]
        public string characterButtons;

        [DataMember]
        public string matchingOfferContainers;

        [DataMember]
        public string alreadyOwnedClass;

        [DataMember]
        public string refreshTimeDiv;

        [DataMember]
        public string storeTypeDropdown;
        
        [DataMember]
        public string storeOptionCredits;

        [DataMember]
        public string storeOptionMarks;


        public XPathConfig()
        {
            platformSignInButton = "//a[@href='/sso/login/steam']";
            steamLoginButton = "//input[@id='imageLogin']";
            characterButtons = "//button[@class='my-button']";
            matchingOfferContainers = "//div[contains(@class,'offer-match')]";
            alreadyOwnedClass = "item-already-owned";
            refreshTimeDiv = "//div[contains(string(), 'Refresh') and contains(@class,'css-13fvtaj')]";

            storeTypeDropdown = "//select[@id='store_type_0']";
            storeOptionCredits = "credits";
            storeOptionMarks = "marks";

        }

        public void SaveConfig()
        {
            DataContractSerializer dcs = new DataContractSerializer( this.GetType() );
            XmlWriterSettings settings = new XmlWriterSettings {Indent = true };

            using (XmlWriter writer = XmlWriter.Create("config_xpath.xml", settings)) {
                dcs.WriteObject(writer, this);
            }

        }

        public bool LoadConfig()
        {
            if(!File.Exists("config_xpath.xml")) {
                return false;
            }

            DataContractSerializer dcs = new DataContractSerializer( this.GetType() );

            using(XmlReader reader = XmlReader.Create("config_xpath.xml")) {
                XPathConfig readConfig = (XPathConfig)dcs.ReadObject(reader);

                this.platformSignInButton = readConfig.platformSignInButton;
                this.steamLoginButton = readConfig.steamLoginButton;
                this.characterButtons = readConfig.characterButtons;
                this.matchingOfferContainers = readConfig.matchingOfferContainers;
                this.alreadyOwnedClass = readConfig.alreadyOwnedClass;
                this.refreshTimeDiv = readConfig.refreshTimeDiv;

                this.storeTypeDropdown = readConfig.storeTypeDropdown;
                this.storeOptionCredits = readConfig.storeOptionCredits;
                this.storeOptionMarks = readConfig.storeOptionMarks;
            }
            
            return true;

        }



    }
}
