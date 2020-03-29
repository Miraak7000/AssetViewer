using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable PossibleNullReferenceException
namespace RDA.Data {
  public class RewardPool {
    #region Properties

    public String ID { get; set; }
    public Int32 Weight { get; set; }
    public List<String> Items { get; set; }

    #endregion Properties

    #region Constructors

    public RewardPool(String id, Int32 weight) {
      this.ID = id;
      this.Weight = weight;
      this.Items = new List<String>();
      this.DiscoverItems(id);
      //var rewardItem = Assets.Original.XPathSelectElement($"//Asset[Values/Standard/GUID={id}]");
      //switch (rewardItem.Element("Template").Value) {
      //  case "QuestItem":
      //    // ignore
      //    break;
      //  case "RewardPool":
      //    break;
      //  case "Product":
      //    this.Items.Add(id);
      //    break;
      //  default:
      //    throw new NotImplementedException(rewardItem.Element("Template").Value);
      //}
    }

    #endregion Constructors

    #region Methods

    public override String ToString() {
      return $"{this.ID} - {this.Weight}";
    }

    public XElement ToXml() {
      var result = new XElement("RP");
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XAttribute("W", this.Weight));
      result.Add(new XElement("I", this.Items.Select(s => new XElement("I", s))));
      return result;
    }

    private void DiscoverItems(String value) {
      var asset = Assets.Original.XPathSelectElement($"//Asset[Values/Standard/GUID={value}]");
      switch (asset.Element("Template").Value) {
        case "RewardPool":
          foreach (var item in asset.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink")) {
            this.DiscoverItems(item.Value);
          }
          break;

        case "QuestItem":
        case "ActiveItem":
        case "ItemSpecialAction":
        case "ItemSpecialActionVisualEffect":
          // TODO: need to be implemented first
          break;

        case "CultureItem":
        case "GuildhouseItem":
        case "HarborOfficeItem":
        case "TownhallItem":
        case "ShipSpecialist":
        case "Product":
        case "VehicleItem":
          if (!this.Items.Contains(value))
            this.Items.Add(value);
          break;

        default:
          //throw new NotImplementedException(asset.Element("Template").Value);
          break;
      }
    }

    #endregion Methods
  }
}