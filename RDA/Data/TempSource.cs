using RDA.Templates;
using RDA.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Diagnostics;

namespace RDA.Data {

  public class TempSource {

    #region Properties
    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public List<Description> Details { get; set; } = new List<Description>();
    #endregion

    #region Constructor
    public TempSource(SourceWithDetails element) {
      var Source = element.Source;
      this.ID = Source.XPathSelectElement("Values/Standard/GUID").Value;
      this.Name = Source.XPathSelectElement("Values/Standard/Name").Value;
      switch (Source.Element("Template").Value) {
        case "TourismFeature":
          this.Text = new Description("Tourism", "Tourismus");
          foreach (var item in element.Details) {
            var cityStatus = item.Element("CityStatus");
            if (cityStatus != null) {
              var desc = new Description((Assets.TourismStati[cityStatus.Value].Element("AttractivenessThreshold")?.Value ?? "0") + " Attractiveness", (Assets.TourismStati[cityStatus.Value].Element("AttractivenessThreshold")?.Value ?? "0") + " Attraktivität");
              Details.Add(desc);
            }
            else if (item.Element("UnlockingSpecialist") != null) {
              Details.Add(new Description(item.Element("UnlockingSpecialist").Value));
            }
            else if (item.Element("UnlockingSetBuff") != null) {
              Details.Add(new Description(item.Element("UnlockingSetBuff").Value));
            }
            else {

            }
          }
          break;
        case "MonumentEventReward":
        case "CollectablePicturePuzzle":
          this.Text = new Description(Source.XPathSelectElement("Values/Standard/GUID").Value);
          this.Details = element.Details.Select(d => new Description(d.XPathSelectElement("Values/Standard/GUID").Value)).ToList();
          break;
        case "Expedition":
          this.Text = new Description(Source.XPathSelectElement("Values/Expedition/ExpeditionName").Value);
          // Processing Details
          foreach (var item in element.Details) {
            // Detail points to Expedition
            if (item.Element("Template")?.Value == "Expedition") {
              var desc = new Description("Expedition Abgeschossen", "Successfully Expedition");
              this.Details.Add(desc);
              continue;
            }
            // Detail points to ExpeditionDecision
            var path = "";
            if (item.Element("Template").Value == "ExpeditionDecision") {
              path = item.XPathSelectElement("Values/Standard/Name").Value.Split(' ').Last();
            }
            // Add Detail
            var parent = item.XPathSelectElement("Values/Standard/GUID").Value.FindParentElement(new[] { "ExpeditionEvent", "Expedition" });
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
              }
            }
            else {
              throw new NotImplementedException();
            }
          }
          break;
        case "Profile_3rdParty":
        case "Profile_3rdParty_Pirate":
        case "HafenHugo":
        case "Harbor":
          this.Text = new Description(Source.XPathSelectElement("Values/Standard/GUID").Value);
          this.Text.EN = $"Harbour - {this.Text.EN}";
          this.Text.DE = $"Hafen - {this.Text.DE}";
          foreach (var item in element.Details) {
            Description desc = null;
            switch (item.Element("Template").Value) {
              case "EarlyGame":
                desc = new Description("Early Game", "Frühes Spiel");
                break;
              case "EarlyMidGame":
                desc = new Description("Early-Mid Game", "Frühes-Mittleres Spiel");
                break;
              case "MidGame":
                desc = new Description("Mid Game", "Mittleres Spiel");
                break;
              case "LateMidGame":
                desc = new Description("Late-Mid Game", "Mittleres-Spätes Spiel");
                break;
              case "LateGame":
                desc = new Description("Late Game", "Spätes Spiel");
                break;
              case "EndGame":
                desc = new Description("EndGame", "End Game");
                break;
            }
            this.Details.Add(desc);
          }
          break;
        case "Quest":
        case "A7_QuestStatusQuo":
        case "A7_QuestEscortObject":
        case "A7_QuestDeliveryObject":
        case "A7_QuestDestroyObjects":
        case "A7_QuestPickupObject":
        case "A7_QuestFollowShip":
        case "A7_QuestItemUsage":
        case "A7_QuestPicturePuzzleObject":
        case "A7_QuestSmuggler":
          this.Text = new Description(Source.XPathSelectElement("Values/Quest/QuestGiver").Value);
          this.Text.EN = $"Quest - {this.Text.EN}";
          this.Text.DE = $"Quest - {this.Text.DE}";
          foreach (var item in element.Details) {
            // Todo: Fehlende Texte??
            try {
              var desc = new Description(item.XPathSelectElement("Values/Standard/GUID").Value);
              this.Details.Add(desc);
            }
            catch (Exception e) {

              var desc = new Description(item.XPathSelectElement("Values/Standard/Name").Value, item.XPathSelectElement("Values/Standard/Name").Value);
              this.Details.Add(desc);
            }
          }
          break;
        case "ShipDrop":
          this.Text = new Description(element.Source.XPathSelectElement("Values/Standard/GUID").Value);
          this.Text.EN = $"Ship Drop - {this.Text.EN}";
          this.Text.DE = $"Schiff Drop - {this.Text.DE}";
          this.Details = element.Details.Select(d => new Description(d.XPathSelectElement("Values/Standard/GUID").Value)).ToList();
          break;
        default:
          throw new NotImplementedException();
      }
      if (this.Text.Icon == null) {
        this.Text.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_skull.png");
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement("Source");
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("Name", this.Name));
      result.Add(this.Text.ToXml("Text"));

      var details = new XElement("Details");
      foreach (var detail in this.Details) {
        details.Add(detail.ToXml("Text"));
      }
      result.Add(details);

      return result;
    }
    #endregion

  }

}