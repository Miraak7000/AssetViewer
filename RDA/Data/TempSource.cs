using System;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {
    public class TempSource {
        #region Properties
        public String ID { get; set; }
        public String Name { get; set; }
        public Icon Icon { get; set; }
        public Description Text { get; set; }
        #endregion

        #region Constructor
        public TempSource(XElement element) {
            this.ID = element.XPathSelectElement("Values/Standard/GUID").Value;
            this.Name = element.XPathSelectElement("Values/Standard/Name").Value;
            switch (element.Element("Template").Value) {
                case "TourismFeature":
                    this.Text = new Description("Tourism", "Tourismus");
                    break;
                case "MonumentEventReward":
                case "CollectablePicturePuzzle":
                    this.Text = new Description(element.XPathSelectElement("Values/Standard/GUID").Value);
                    break;
                case "Expedition":
                    this.Text = new Description(element.XPathSelectElement("Values/Expedition/ExpeditionName").Value);
                    break;
                case "Profile_3rdParty":
                case "Profile_3rdParty_Pirate":
                    this.Text = new Description(element.XPathSelectElement("Values/Standard/GUID").Value);
                    this.Text.EN = $"Harbour - {this.Text.EN}";
                    this.Text.DE = $"Hafen - {this.Text.DE}";
                    break;
                case "Quest":
                case "A7_QuestEscortObject":
                case "A7_QuestDeliveryObject":
                case "A7_QuestDestroyObjects":
                case "A7_QuestPickupObject":
                case "A7_QuestFollowShip":
                case "A7_QuestItemUsage":
                    this.Text = new Description(element.XPathSelectElement("Values/Quest/QuestGiver").Value);
                    this.Text.EN = $"Quest - {this.Text.EN}";
                    this.Text.DE = $"Quest - {this.Text.DE}";
                    break;
                default:
                    break;
            }
            if (element.XPathSelectElement("Values/Standard/IconFilename") != null) {
                this.Icon = new Icon(element.XPathSelectElement("Values/Standard/IconFilename").Value);
            }
            else {
                this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_skull.png");
            }
        }
        #endregion

        #region Public Methods
        public XElement ToXml() {
            var result = new XElement("Source");
            result.Add(new XAttribute("ID", this.ID));
            result.Add(new XElement("Name", this.Name));
            result.Add(this.Icon == null ? new XElement("Icon") : this.Icon.ToXml());
            result.Add(this.Text.ToXml("Text"));
            return result;
        }
        #endregion

    }
}