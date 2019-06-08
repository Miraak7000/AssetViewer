using RDA.Library;
using RDA.Veras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {
    public class VerasTempSource : TempSource {
        #region Constructors

        public VerasTempSource(RootWithDetails element) : base(element.Root) {
            switch (element.Root.Element("Template").Value) {
                case "TourismFeature":
                case "MonumentEventReward":
                case "CollectablePicturePuzzle":
                    break;

                case "Expedition":
                    foreach (var item in element.Details) {
                        if (item.Name == "ExpeditionEvent") {
                            Console.WriteLine("");
                        }

                        if (item.Element("Template")?.Value == "Expedition") {
                            var desc = new Description("Expedition Abgeschossen", " Successfully Expedition");
                            this.Details.Add(desc);
                            continue;
                        }

                        var path = "";
                        if (item.Element("Template").Value == "ExpeditionDecision") {
                            path = item.XPathSelectElement("Values/Standard/Name").Value.Split(' ').Last();
                        }
                        var parent = item.XPathSelectElement("Values/Standard/GUID").Value.VerasFindParent(new[] { "ExpeditionEvent", "Expedition" });
                        if (parent?.Element("Template") != null) {
                            switch (parent.Element("Template").Value) {
                                case "ExpeditionEvent":
                                    var desc = new Description(parent.XPathSelectElement("Values/Standard/GUID").Value);
                                    desc.DE = $"{desc.DE} {path}";
                                    desc.EN = $"{desc.EN} {path}";
                                    this.Details.Add(desc);
                                    break;

                                case "Expedition":
                                    desc = new Description("Expedition Abgeschossen", " Successfully Expedition");
                                    this.Details.Add(desc);
                                    break;

                                default:
                                    break;
                            }
                        }
                        else {
                            Console.WriteLine("");
                        }
                    }
                    break;

                case "Profile_3rdParty":
                case "Profile_3rdParty_Pirate":
                case "Quest":
                case "A7_QuestEscortObject":
                case "A7_QuestDeliveryObject":
                case "A7_QuestDestroyObjects":
                case "A7_QuestPickupObject":
                case "A7_QuestFollowShip":
                case "A7_QuestItemUsage":
                    foreach (var item in element.Details) {
                        var desc = new Description(item.XPathSelectElement("Values/Standard/GUID").Value);
                        this.Details.Add(desc);
                    }
                    break;
                case "ShipDrop":
                    this.Text = new Description(element.Root.XPathSelectElement("Values/Standard/GUID").Value);
                    this.Text.EN = $"Ship Drop - {this.Text.EN}";
                    this.Text.DE = $"Schiff Drop - {this.Text.DE}";
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion Constructors

        #region Properties

        public List<Description> Details { get; set; } = new List<Description>();

        #endregion Properties

        #region Methods

        public new XElement ToXml() {
            var result = base.ToXml();
            var details = new XElement("Details");
            foreach (var detail in this.Details) {
                details.Add(detail.ToXml("Text"));
            }

            result.Add(details);
            return result;
        }

        #endregion Methods
    }
}