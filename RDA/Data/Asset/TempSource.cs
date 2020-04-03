using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class TempSource {

    #region Public Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public List<(Description desc, double weight)> Details { get; set; } = new List<(Description, double)>();

    #endregion Public Properties

    #region Public Constructors

    public TempSource(SourceWithDetails element) {
      var Source = element.Source;
      this.ID = Source.XPathSelectElement("Values/Standard/GUID").Value;
      this.Name = Source.XPathSelectElement("Values/Standard/Name").Value;
      switch (Source.Element("Template").Value) {
        case "TourismFeature":
          this.Text = new Description("-4");
          foreach (var item in element.Details) {
            var cityStatus = item.Asset.Element("Item")?.Element("CityLevel");
            if (cityStatus != null) {
              var desc = new Description("145011")
                .InsertBefore(Assets.TourismThresholds[cityStatus.Value])
                .AppendInBraces(new Description(Assets.KeyToIdDict[item.Asset.Element("Template").Value]));
              Details.Add((desc, item.Weight));
            }
            else if (item.Asset.Element("Item")?.Element("UnlockingSpecialist") != null) {
              Details.Add((new Description(item.Asset.Element("Item").Element("UnlockingSpecialist").Value), item.Weight));
            }
            else if (item.Asset.Element("Item")?.Element("UnlockingSetBuff") != null) {
              Details.Add((new Description(item.Asset.Element("Item").Element("UnlockingSetBuff").Value), item.Weight));
            }
            else if (item.Asset.Element("Template")?.Value == "HarborOfficeItem" || item.Asset.Element("Template")?.Value == "CultureBuff") {
              Details.Add((new Description(item.Asset.XPathSelectElement("Values/Standard/GUID").Value), item.Weight));
            }
            else {
              throw new NotImplementedException();
            }
          }
          break;

        case "ItemWithUI":
        case "MonumentEventReward":
        case "CollectablePicturePuzzle":
          this.Text = new Description(Source.XPathSelectElement("Values/Standard/GUID").Value);
          this.Details = element.Details.Select(d => (new Description(d.Asset.XPathSelectElement("Values/Standard/GUID").Value), d.Weight)).ToList();
          break;

        case "Expedition":
          this.Text = new Description(Source.XPathSelectElement("Values/Expedition/ExpeditionName").Value);
          // Processing Details
          foreach (var item in element.Details) {
            // Detail points to Expedition
            if (item.Asset.Element("Template").Value == "Expedition") {
              Description difficulty = null;
              switch (item.Asset.XPathSelectElement("Values/Expedition/ExpeditionDifficulty")?.Value) {
                case "Easy":
                  difficulty = new Description("11031");
                  break;

                case "Average":
                  difficulty = new Description("11032");
                  break;

                case "Hard":
                  difficulty = new Description("11033");
                  break;

                default:
                  difficulty = new Description("11031");
                  break;
              }
              var desc = new Description("-5").AppendWithSpace("-->").AppendWithSpace(difficulty);
              this.Details.Add((desc, item.Weight));
              continue;
            }
            // Detail points to Expedition Event
            else if (item.Asset.Element("Asset").Element("Template").Value == "ExpeditionEvent") {
              var desc = new Description(item.Asset.XPathSelectElement("Values/Standard/GUID").Value).AppendWithSpace(item.Asset.Element("Template").Value);
              this.Details.Add((desc, item.Weight));
            }
            else {
              throw new NotImplementedException();
            }
          }
          break;

        //case "Profile_3rdParty":
        //case "Profile_3rdParty_Pirate":
        //case "HafenHugo":
        case "Harbor":
          this.Text = new Description(Source.XPathSelectElement("Values/Standard/GUID").Value).InsertBefore("-").InsertBefore(new Description("11150"));
          foreach (var item in element.Details) {
            var desc = GetDescriptionFromProgression(item.Asset.Element("Template").Value);
            this.Details.Add((desc, item.Weight));
          }
          break;

        case "TakeOver":
          this.Text = new Description(Source.XPathSelectElement("Values/Standard/GUID").Value).InsertBefore("-").InsertBefore(new Description("10839"));
          foreach (var item in element.Details) {
            var details = item.Asset.Element("Template").Value.Split('#');
            var progression = GetDescriptionFromProgression(details[1]);
            Description desc = null;
            switch (details[0]) {
              case "MainIslandRewardPool":
                desc = new Description("-1240").AppendWithSpace(progression);
                break;

              case "SecondaryIslandRewardPool":
                desc = new Description("-1241").AppendWithSpace(progression);
                break;

              default:
                throw new NotImplementedException();
            }
            this.Details.Add((desc, item.Weight));
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
        case "A7_QuestLostCargo":
        case "A7_QuestExpedition":
        case "A7_QuestDivingBellSonar":
        case "A7_QuestDivingBellTreasureMap":
          var questgiver = Source.XPathSelectElement("Values/Quest/QuestGiver")?.Value;
          if (questgiver != null) {
            this.Text = new Description(questgiver).InsertBefore("-").InsertBefore(new Description("2734"));
            foreach (var item in element.Details) {
              this.Details.Add((new Description(item.Asset.XPathSelectElement("Values/Standard/GUID").Value), item.Weight));
            }

          }
          else {

          }
          break;

        case "ShipDrop":
          this.Text = new Description(element.Source.XPathSelectElement("Values/Standard/GUID").Value).InsertBefore("-").InsertBefore(new Description("-12"));
          this.Details = element.Details.Select(d => (new Description(d.Asset.XPathSelectElement("Values/Standard/GUID").Value), d.Weight)).ToList();
          break;

        case "Crafting":
          this.Text = new Description(element.Source.XPathSelectElement("Values/Standard/GUID").Value).InsertBefore("-").InsertBefore(new Description("112529"));
          this.Details = element.Details.Select(d => (new Description(d.Asset.XPathSelectElement("Values/Standard/GUID").Value).AppendInBraces(new Description(Assets
            .KeyToIdDict[d.Asset.Element("Template").Value])), d.Weight)).ToList();
          break;

        case "Dive":
          this.Text = new Description("113420");
          foreach (var item in element.Details) {
            var desc = new Description(item.Asset.XPathSelectElement("Values/Standard/GUID").Value);
            if (item.Asset.Element("Template").Value == "ItemWithUI") {
              desc = desc.AppendInBraces(new Description(item.Asset.Descendants("TreasureSessionOrRegion").First().Value));
            }
            this.Details.Add((desc, item.Weight));
          }
          break;

        case "Pickup":
          this.Text = new Description("500334");
          this.Details = element.Details.Select(i => (new Description("500334"), i.Weight)).ToList();
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

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("S");
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XAttribute("N", this.Name));
      result.Add(this.Text.ToXml("T"));

      var details = new XElement("DL");
      foreach (var detail in this.Details) {
        var xDetail = new XElement("D");

        xDetail.Add(detail.desc.ToXml("T"));
        var weight = (detail.weight * 100.0F).Round();
        xDetail.Add(new XAttribute("W", weight));
        details.Add(xDetail);
      }
      result.Add(details);

      return result;
    }

    #endregion Public Methods

    #region Private Methods

    private static Description GetDescriptionFromProgression(string progression) {
      Description desc = null;
      switch (progression) {
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

        default:
          throw new NotImplementedException();
      }
      return desc;
    }

    #endregion Private Methods
  }
}