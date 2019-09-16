using RDA.Library;
using RDA.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class TempSource {

    #region Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public List<Description> Details { get; set; } = new List<Description>();

    #endregion Properties

    #region Constructors

    public TempSource(SourceWithDetails element) {
     
      var Source = element.Source;
      this.ID = Source.XPathSelectElement("Values/Standard/GUID").Value;
      this.Name = Source.XPathSelectElement("Values/Standard/Name").Value;
      switch (Source.Element("Template").Value) {
        case "TourismFeature":
          this.Text = new Description("-4");
          foreach (var item in element.Details) {
            var cityStatus = item.Element("CityStatus");
            if (cityStatus != null) {
              var desc = new Description("145011").InsertBefore((Assets.TourismStati[cityStatus.Value].Element("AttractivenessThreshold")?.Value ?? "0"));
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

        case "ItemWithUI":
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
              var desc = new Description("-5");
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
                  var desc = new Description(parent.XPathSelectElement("Values/Standard/GUID").Value).Append(path);
                  this.Details.Add(desc);
                  break;

                case "Expedition":
                  desc = new Description("-5");
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
          this.Text = new Description(Source.XPathSelectElement("Values/Standard/GUID").Value).InsertBefore("-").InsertBefore(new Description("11150"));
          foreach (var item in element.Details) {
            Description desc = null;
            switch (item.Element("Template").Value) {
              case "EarlyGame":
                desc = new Description("-6");
                break;

              case "EarlyMidGame":
                desc = new Description("-7");
                break;

              case "MidGame":
                desc = new Description("-8");
                break;

              case "LateMidGame":
                desc = new Description("-9");
                break;

              case "LateGame":
                desc = new Description("-10");
                break;

              case "EndGame":
                desc = new Description("-11");
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
        case "A7_QuestDivingBellGeneric":
        case "A7_QuestSelectObject":
        case "A7_QuestPhotography":
        case "A7_QuestSustain":
        case "A7_QuestNewspaperArticle":
          var questgiver = Source.XPathSelectElement("Values/Quest/QuestGiver").Value;
          this.Text = new Description(questgiver).InsertBefore("-").InsertBefore(new Description("2734"));
          foreach (var item in element.Details.Select(e => new Description(e.XPathSelectElement("Values/Standard/GUID").Value)).Distinct()) {
            this.Details.Add(item);
          }
          break;

        case "A7_QuestDivingBellSonar":
        case "A7_QuestDivingBellTreasureMap":
          questgiver = Source.XPathSelectElement("Values/Quest/QuestGiver")?.Value;
          this.Text = new Description(questgiver ?? "113420").InsertBefore("-").InsertBefore(new Description("2734"));
          foreach (var item in element.Details.Select(e => new Description(e.XPathSelectElement("Values/Standard/GUID").Value)).Distinct()) {
            this.Details.Add(item);
          }
          break;

        case "ShipDrop":
          this.Text = new Description(element.Source.XPathSelectElement("Values/Standard/GUID").Value).InsertBefore("-").InsertBefore(new Description("-12"));
          this.Details = element.Details.Select(d => new Description(d.XPathSelectElement("Values/Standard/GUID").Value)).ToList();
          break;

        case "Crafting":
          this.Text = new Description(element.Source.XPathSelectElement("Values/Standard/GUID").Value).InsertBefore("-").InsertBefore(new Description("112529"));
          this.Details = element.Details.Select(d => new Description(d.XPathSelectElement("Values/Standard/GUID").Value)).ToList();
          break;

        case "Dive":
          this.Text = new Description("113420");
          this.Details = element.Details.Select(_ => new Description("113420")).ToList();
          break;
        case "Item":
          this.Text = new Description(Source.XPathSelectElement("Values/Standard/GUID").Value).InsertBefore("-").InsertBefore(new Description("-101"));
          break;

        default:
          Debug.WriteLine(Source.Element("Template").Value);
          throw new NotImplementedException();
      }
      if (this.Text.Icon == null) {
        this.Text.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_skull.png");
      }
    }

    #endregion Constructors

    #region Methods

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

    #endregion Methods
  }
}