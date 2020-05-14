using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable PossibleNullReferenceException
namespace RDA.Data {

  public class RewardPool {

    #region Public Properties

    public string ID { get; set; }
    public int Weight { get; set; }
    public List<string> Items { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public RewardPool(string id, int weight) {
      ID = id;
      Weight = weight;
      Items = new List<string>();
      DiscoverItems(id);
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

    #endregion Public Constructors

    #region Public Methods

    public override string ToString() {
      return $"{ID} - {Weight}";
    }

    public XElement ToXml() {
      var result = new XElement("RP");
      result.Add(new XAttribute("ID", ID));
      result.Add(new XAttribute("W", Weight));
      result.Add(new XElement("I", Items.Select(s => new XElement("I", s))));
      return result;
    }

    #endregion Public Methods

    #region Private Methods

    private void DiscoverItems(string value) {
      var asset = Assets.Original.XPathSelectElement($"//Asset[Values/Standard/GUID={value}]");
      switch (asset.Element("Template").Value) {
        case "RewardPool":
          foreach (var item in asset.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink")) {
            DiscoverItems(item.Value);
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
          if (!Items.Contains(value))
            Items.Add(value);
          break;

        default:
          //throw new NotImplementedException(asset.Element("Template").Value);
          break;
      }
    }

    #endregion Private Methods
  }
}